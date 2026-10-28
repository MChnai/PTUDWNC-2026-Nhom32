-- ============================================================================
-- CulinaryBlog PostgreSQL 16 schema: compatible with the supplied 2026 backup
-- and with the EF Core mappings supplied earlier.
--
-- Safe to re-run. Does not insert demo users, fake password hashes or recipes.
-- Does not delete data or drop tables. Test against a restored copy first.
--
-- IMPORTANT: This script defines/adapts SCHEMA, not CONTENT.
-- For the 100 English recipes and their related content, restore the supplied
-- culinaryblog_backup.dump, then run culinaryblog_english_content_migration.sql.
-- Do not run automatic InitialCreate EF migrations against a manually-created
-- schema without first reconciling __EFMigrationsHistory.
-- ============================================================================

BEGIN;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp" WITH SCHEMA public;
CREATE EXTENSION IF NOT EXISTS "unaccent" WITH SCHEMA public;
CREATE EXTENSION IF NOT EXISTS "pg_trgm" WITH SCHEMA public;

-- Recreate the search configuration used by the actual dump if absent.
-- It also works with English-only recipe titles and descriptions.
DO $fts$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_catalog.pg_ts_config c
        JOIN pg_catalog.pg_namespace n ON n.oid = c.cfgnamespace
        WHERE n.nspname = 'public' AND c.cfgname = 'vietnamese_unaccent'
    ) THEN
        CREATE TEXT SEARCH CONFIGURATION public.vietnamese_unaccent
            (COPY = pg_catalog.simple);
        ALTER TEXT SEARCH CONFIGURATION public.vietnamese_unaccent
            ALTER MAPPING FOR word, hword, hword_part
            WITH public.unaccent, pg_catalog.simple;
    END IF;
END
$fts$;

-- Application / EF Core migration metadata. No fake migration rows inserted.
CREATE TABLE IF NOT EXISTS public."__EFMigrationsHistory" (
    "MigrationId" VARCHAR(150) PRIMARY KEY,
    "ProductVersion" VARCHAR(32) NOT NULL
);

CREATE TABLE IF NOT EXISTS public.users (
    id UUID PRIMARY KEY DEFAULT public.uuid_generate_v4(),
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(150) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    role VARCHAR(30) NOT NULL DEFAULT 'Author',
    avatar_url VARCHAR(500),
    bio TEXT,
    email_confirmed BOOLEAN NOT NULL DEFAULT TRUE,
    access_failed_count INT NOT NULL DEFAULT 0,
    lockout_end TIMESTAMPTZ,
    lockout_enabled BOOLEAN NOT NULL DEFAULT TRUE,
    google_provider_key VARCHAR(255),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_at TIMESTAMPTZ,
    CONSTRAINT chk_users_role CHECK (role IN ('Admin', 'Author'))
);

CREATE TABLE IF NOT EXISTS public.refresh_tokens (
    id UUID PRIMARY KEY DEFAULT public.uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES public.users(id) ON DELETE CASCADE,
    token VARCHAR(500) NOT NULL UNIQUE,
    expires_at TIMESTAMPTZ NOT NULL,
    created_by_ip VARCHAR(50),
    is_revoked BOOLEAN NOT NULL DEFAULT FALSE,
    revoked_at TIMESTAMPTZ,
    revoked_by_ip VARCHAR(50),
    replaced_by_token VARCHAR(500),
    reason_revoked VARCHAR(255),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_at TIMESTAMPTZ
);

-- Present in the backup; missing from the old handwritten schema.
CREATE TABLE IF NOT EXISTS public.user_logins (
    login_provider VARCHAR(128) NOT NULL,
    provider_key VARCHAR(255) NOT NULL,
    provider_display_name VARCHAR(255),
    user_id UUID NOT NULL REFERENCES public.users(id) ON DELETE CASCADE,
    CONSTRAINT user_logins_pkey PRIMARY KEY (login_provider, provider_key)
);

CREATE TABLE IF NOT EXISTS public.categories (
    id UUID PRIMARY KEY DEFAULT public.uuid_generate_v4(),
    name VARCHAR(100) NOT NULL UNIQUE,
    slug VARCHAR(120) NOT NULL UNIQUE,
    description TEXT,
    image_url VARCHAR(500),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_at TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS public.recipes (
    id UUID PRIMARY KEY DEFAULT public.uuid_generate_v4(),
    title VARCHAR(255) NOT NULL,
    slug VARCHAR(280) NOT NULL UNIQUE,
    description TEXT,
    instructions TEXT,
    notes TEXT,
    prep_time_minutes INT NOT NULL DEFAULT 0,
    cook_time_minutes INT NOT NULL DEFAULT 0,
    servings INT NOT NULL DEFAULT 4,
    difficulty VARCHAR(20) NOT NULL DEFAULT 'Medium',
    status VARCHAR(20) NOT NULL DEFAULT 'Draft',
    published_at TIMESTAMPTZ,
    view_count INT NOT NULL DEFAULT 0,
    rating_average NUMERIC(3,2) NOT NULL DEFAULT 0.00,
    rating_count INT NOT NULL DEFAULT 0,
    category_id UUID NOT NULL REFERENCES public.categories(id) ON DELETE RESTRICT,
    author_id UUID NOT NULL REFERENCES public.users(id) ON DELETE RESTRICT,
    calories INT NOT NULL DEFAULT 0,
    protein NUMERIC(6,2) NOT NULL DEFAULT 0.00,
    carbohydrates NUMERIC(6,2) NOT NULL DEFAULT 0.00,
    fat NUMERIC(6,2) NOT NULL DEFAULT 0.00,
    fiber NUMERIC(6,2),
    sugar NUMERIC(6,2),
    sodium NUMERIC(8,2),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_at TIMESTAMPTZ,

    -- DO NOT add "xmin xid". xmin is a PostgreSQL system column;
    -- EF Core/Npgsql may map it as a concurrency token without declaring it.
    search_vector TSVECTOR GENERATED ALWAYS AS (
        to_tsvector(
            'public.vietnamese_unaccent'::regconfig,
            COALESCE(title, '') || ' ' ||
            COALESCE(description, '') || ' ' ||
            COALESCE(instructions, '')
        )
    ) STORED,
    CONSTRAINT chk_recipes_difficulty CHECK (difficulty IN ('Easy', 'Medium', 'Hard')),
    CONSTRAINT chk_recipes_status CHECK (status IN ('Draft', 'Published', 'Archived')),
    CONSTRAINT chk_recipes_positive_times CHECK (
        prep_time_minutes >= 0 AND cook_time_minutes >= 0 AND servings > 0
    )
);

CREATE TABLE IF NOT EXISTS public.recipe_ingredients (
    id UUID PRIMARY KEY DEFAULT public.uuid_generate_v4(),
    recipe_id UUID NOT NULL REFERENCES public.recipes(id) ON DELETE CASCADE,
    name VARCHAR(150) NOT NULL,
    quantity NUMERIC(8,2) NOT NULL,
    unit VARCHAR(50) NOT NULL,
    notes VARCHAR(255),
    -- The old EF Core RecipeIngredientConfiguration maps OrderIndex here.
    order_index INT NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_at TIMESTAMPTZ,
    CONSTRAINT recipe_ingredients_quantity_check CHECK (quantity > 0)
);

CREATE TABLE IF NOT EXISTS public.recipe_steps (
    id UUID PRIMARY KEY DEFAULT public.uuid_generate_v4(),
    recipe_id UUID NOT NULL REFERENCES public.recipes(id) ON DELETE CASCADE,
    step_number INT NOT NULL,
    title VARCHAR(200),
    description TEXT NOT NULL,
    duration_minutes INT,
    timer_minutes INT,
    image_url VARCHAR(500),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_at TIMESTAMPTZ,
    CONSTRAINT recipe_steps_step_number_check CHECK (step_number > 0),
    CONSTRAINT recipe_steps_duration_minutes_check CHECK (
        duration_minutes IS NULL OR duration_minutes >= 0
    ),
    CONSTRAINT uq_recipe_step_number UNIQUE (recipe_id, step_number)
        DEFERRABLE INITIALLY DEFERRED
);

CREATE TABLE IF NOT EXISTS public.recipe_images (
    id UUID PRIMARY KEY DEFAULT public.uuid_generate_v4(),
    recipe_id UUID NOT NULL REFERENCES public.recipes(id) ON DELETE CASCADE,
    original_url VARCHAR(500) NOT NULL,
    thumbnail_url VARCHAR(500),
    medium_url VARCHAR(500),
    alt_text VARCHAR(200),
    is_primary BOOLEAN NOT NULL DEFAULT FALSE,
    display_order INT NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_at TIMESTAMPTZ
);

-- Adapt restored databases to the previously supplied EF Core mapping.
-- Preserves order data and runs the rename only once, if needed.
DO $ingredients$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public'
          AND table_name = 'recipe_ingredients'
          AND column_name = 'sort_order'
    ) AND NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public'
          AND table_name = 'recipe_ingredients'
          AND column_name = 'order_index'
    ) THEN
        ALTER TABLE public.recipe_ingredients
            RENAME COLUMN sort_order TO order_index;
    END IF;
END
$ingredients$;

-- Add optional columns missing in the old schema without touching values.
ALTER TABLE public.refresh_tokens
    ADD COLUMN IF NOT EXISTS reason_revoked VARCHAR(255);
ALTER TABLE public.recipe_images
    ADD COLUMN IF NOT EXISTS medium_url VARCHAR(500);
ALTER TABLE public.recipe_steps
    ADD COLUMN IF NOT EXISTS duration_minutes INT,
    ADD COLUMN IF NOT EXISTS timer_minutes INT;

-- Backup constraints not fully present in the old handwritten schema.
DO $constraints$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_catalog.pg_constraint
        WHERE conrelid = 'public.categories'::regclass
          AND conname = 'categories_name_key'
    ) THEN
        ALTER TABLE public.categories
            ADD CONSTRAINT categories_name_key UNIQUE (name);
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_catalog.pg_constraint
        WHERE conrelid = 'public.recipe_steps'::regclass
          AND conname = 'uq_recipe_step_number'
    ) THEN
        ALTER TABLE public.recipe_steps
            ADD CONSTRAINT uq_recipe_step_number UNIQUE (recipe_id, step_number)
            DEFERRABLE INITIALLY DEFERRED;
    END IF;
END
$constraints$;

CREATE INDEX IF NOT EXISTS idx_refresh_tokens_token
    ON public.refresh_tokens(token);
CREATE INDEX IF NOT EXISTS idx_refresh_tokens_user_id
    ON public.refresh_tokens(user_id);
CREATE INDEX IF NOT EXISTS idx_user_logins_user_id
    ON public.user_logins(user_id);
CREATE INDEX IF NOT EXISTS idx_recipes_search_vector
    ON public.recipes USING GIN(search_vector);
CREATE INDEX IF NOT EXISTS idx_recipes_category_id
    ON public.recipes(category_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS idx_recipes_author_id
    ON public.recipes(author_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS idx_recipes_status
    ON public.recipes(status);
CREATE INDEX IF NOT EXISTS idx_recipes_published_at
    ON public.recipes(published_at DESC);
CREATE INDEX IF NOT EXISTS idx_recipes_status_published_at
    ON public.recipes(status, published_at DESC) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS idx_recipes_cook_time
    ON public.recipes(cook_time_minutes);
CREATE INDEX IF NOT EXISTS idx_recipes_difficulty
    ON public.recipes(difficulty);
CREATE INDEX IF NOT EXISTS idx_recipe_ingredients_recipe_id
    ON public.recipe_ingredients(recipe_id);
CREATE INDEX IF NOT EXISTS idx_recipe_steps_recipe_id
    ON public.recipe_steps(recipe_id, step_number);
CREATE INDEX IF NOT EXISTS idx_recipe_images_recipe_id
    ON public.recipe_images(recipe_id);

CREATE OR REPLACE FUNCTION public.update_updated_at_column()
RETURNS trigger LANGUAGE plpgsql AS $function$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END
$function$;

-- Add each timestamp trigger only if it is not already installed.
DO $triggers$
DECLARE
    tbl TEXT;
    trigger_name TEXT;
BEGIN
    FOREACH tbl IN ARRAY ARRAY[
        'users', 'refresh_tokens', 'categories', 'recipes',
        'recipe_ingredients', 'recipe_steps', 'recipe_images'
    ] LOOP
        trigger_name := 'trg_' || tbl || '_updated_at';
        IF NOT EXISTS (
            SELECT 1
            FROM pg_catalog.pg_trigger t
            JOIN pg_catalog.pg_class c ON c.oid = t.tgrelid
            JOIN pg_catalog.pg_namespace n ON n.oid = c.relnamespace
            WHERE n.nspname = 'public'
              AND c.relname = tbl
              AND t.tgname = trigger_name
              AND NOT t.tgisinternal
        ) THEN
            EXECUTE format(
                'CREATE TRIGGER %I BEFORE UPDATE ON public.%I '
                || 'FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column()',
                trigger_name, tbl
            );
        END IF;
    END LOOP;
END
$triggers$;

COMMIT;

-- Non-mutating verification queries:
SELECT table_name
FROM information_schema.tables
WHERE table_schema = 'public' AND table_type = 'BASE TABLE'
ORDER BY table_name;

SELECT column_name, data_type
FROM information_schema.columns
WHERE table_schema = 'public' AND table_name = 'recipe_ingredients'
  AND column_name IN ('order_index', 'sort_order')
ORDER BY column_name;
