-- Additive upgrade for the supplied PostgreSQL schema. Back up first.
-- It preserves existing accounts, recipes and uploads. Safe to run again.
BEGIN;
CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS unaccent;
CREATE EXTENSION IF NOT EXISTS pg_trgm;
ALTER TABLE public.recipe_images ADD COLUMN IF NOT EXISTS medium_url varchar(500);
-- Existing valid token sessions survive; raw tokens are replaced by their digests.
UPDATE public.refresh_tokens SET token = encode(digest(token, 'sha256'), 'hex')
WHERE token !~ '^[0-9a-f]{64}$';
UPDATE public.refresh_tokens SET replaced_by_token = encode(digest(replaced_by_token, 'sha256'), 'hex')
WHERE replaced_by_token IS NOT NULL AND replaced_by_token !~ '^[0-9a-f]{64}$';
-- Reordering/removing steps is atomic, including temporary order collisions.
DO $upgrade$
DECLARE constraint_name text;
BEGIN
    FOR constraint_name IN
        SELECT conname FROM pg_constraint WHERE conrelid = 'public.recipe_steps'::regclass
        AND contype = 'u' AND pg_get_constraintdef(oid) LIKE '%recipe_id, step_number%'
    LOOP
        EXECUTE format('ALTER TABLE public.recipe_steps DROP CONSTRAINT %I', constraint_name);
    END LOOP;
    ALTER TABLE public.recipe_steps ADD CONSTRAINT uq_recipe_step_number UNIQUE (recipe_id, step_number) DEFERRABLE INITIALLY DEFERRED;
END
$upgrade$;
CREATE INDEX IF NOT EXISTS idx_recipes_search_vector ON public.recipes USING gin(search_vector);
CREATE INDEX IF NOT EXISTS idx_recipes_public_created ON public.recipes(created_at DESC, id) WHERE status='Published' AND NOT is_deleted;
-- A view counter update must not change the article's editorial last-modified date.
CREATE OR REPLACE FUNCTION public.update_updated_at_column()
RETURNS trigger LANGUAGE plpgsql AS $function$
BEGIN
    IF TG_TABLE_NAME = 'recipes' AND
       (to_jsonb(NEW) -> 'view_count') IS DISTINCT FROM (to_jsonb(OLD) -> 'view_count') AND
       (to_jsonb(NEW) - 'view_count' - 'updated_at') IS NOT DISTINCT FROM
       (to_jsonb(OLD) - 'view_count' - 'updated_at') THEN
        NEW.updated_at = OLD.updated_at;
    ELSE
        NEW.updated_at = NOW();
    END IF;
    RETURN NEW;
END
$function$;
COMMIT;
