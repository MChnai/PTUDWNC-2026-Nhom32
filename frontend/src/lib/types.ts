export interface Category { id: string; name: string; slug: string; description?: string; recipeCount: number }
export interface RecipeSummary {
  id: string; title: string; slug: string; summary?: string; coverImage?: string;
  prepTimeMinutes: number; cookTimeMinutes: number; servings: number; difficulty: number;
  categoryId: string; categoryName: string; authorId: string; authorName: string;
  viewCount: number; averageRating: number; ratingCount: number; createdAt: string; publishedAt: string;
  status?: number;
}
export interface Ingredient { id?: string; name: string; quantity: number; unit: string; notes?: string; orderIndex: number }
export interface Step { id?: string; stepNumber?: number; title: string; description: string; timerMinutes?: number | null; imageUrl?: string | null }
export interface RecipeImage { id: string; originalUrl: string; thumbnailUrl?: string; altText?: string; isPrimary: boolean; orderIndex: number }
export interface Nutrition { calories: number; protein: number; carbohydrates: number; fat: number; fiber?: number | null; sugar?: number | null; sodium?: number | null }
export interface RecipeDetail {
  id: string; title: string; slug: string; description?: string; instructions?: string; notes?: string;
  prepTimeMinutes: number; cookTimeMinutes: number; servings: number; difficulty: number; status: number;
  categoryId: string; category?: Pick<Category, 'id' | 'name' | 'slug'>; authorId: string;
  author?: { id: string; displayName: string; avatarUrl?: string; bio?: string };
  ingredients: Ingredient[]; steps: Step[]; images: RecipeImage[]; nutrition?: Nutrition;
  viewCount: number; ratingAverage: number; ratingCount: number; rowVersion: number;
  createdAt: string; updatedAt?: string; publishedAt?: string;
}
export interface PageResult<T> { items: T[]; totalCount: number; pageNumber: number; pageSize: number; totalPages: number; hasNextPage?: boolean; hasPreviousPage?: boolean }
export interface ManagedResult extends PageResult<RecipeSummary> { draftCount: number; publishedCount: number; archivedCount: number }
export interface User { id: string; fullName: string; email: string; userName: string; avatarUrl?: string; roles: string[]; emailConfirmed?: boolean }
export interface ApiEnvelope<T> { isSuccess: boolean; value: T; errorCode?: string; errorMessage?: string }
