import { apiClient } from './api-client';
import type { Category } from '@/types';

interface ApiResult<T> {
  isSuccess: boolean;
  value: T;
  errorCode?: string;
  errorMessage?: string;
}

export async function getCategories(): Promise<Category[]> {
  const response = await apiClient.get<ApiResult<Category[]>>(
    '/categories'
  );

  if (!response.data.isSuccess) {
    throw new Error(
      response.data.errorMessage || 'Cannot load categories'
    );
  }

  return response.data.value;
}