import 'server-only';
import { cache } from 'react';
import type { Category, PageResult, RecipeSummary, RecipeDetail } from './types';
export const siteUrl = () => (process.env.NEXT_PUBLIC_SITE_URL || 'http://localhost:3000').replace(/\/$/, '');
export async function serverApi<T>(path: string): Promise<T> {
  const response = await fetch(`${process.env.BACKEND_URL || 'http://127.0.0.1:5000'}/api/v1${path}`, {
    cache: 'no-store', signal: AbortSignal.timeout(8000),
  });
  if (!response.ok) throw Object.assign(new Error('Không thể tải nội dung từ máy chủ.'), { status: response.status });
  const data = await response.json();
  if (data.isSuccess === false) throw Object.assign(new Error(data.errorMessage), { status: 404 });
  return data.isSuccess === true ? data.value : data;
}
export const getCategories = cache(() => serverApi<Category[]>('/categories'));
export const getRecipe = cache((slug: string) => serverApi<RecipeDetail>(`/recipes/by-slug/${encodeURIComponent(slug)}`));
export const getRecipes = (query = '') => serverApi<PageResult<RecipeSummary>>(`/recipes${query ? `?${query}` : ''}`);
