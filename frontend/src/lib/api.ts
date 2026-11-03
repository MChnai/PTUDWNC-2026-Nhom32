export class ApiError extends Error {
  constructor(message: string, public status: number, public errors?: Record<string, string[]>) { super(message); }
}
export async function api<T>(path: string, options: RequestInit = {}): Promise<T> {
  let response: Response;
  try {
    response = await fetch(`/api/v1${path}`, { ...options, credentials: 'same-origin', cache: 'no-store',
      headers: { ...(options.body instanceof FormData ? {} : { 'Content-Type': 'application/json' }), ...options.headers } });
  } catch { throw new ApiError('Chưa kết nối được máy chủ. Vui lòng kiểm tra mạng và thử lại.', 0); }
  if (response.status === 204) return undefined as T;
  const data = await response.json().catch(() => ({}));
  if (!response.ok || data.isSuccess === false) {
    const details = data.errors ? Object.values(data.errors as Record<string, string[]>).flat().join(' ') : '';
    throw new ApiError(details || data.detail || data.errorMessage || data.message || 'Không thể hoàn tất yêu cầu. Vui lòng thử lại.', response.status, data.errors);
  }
  return (data.isSuccess === true ? data.value : data) as T;
}
export const json = (data: unknown) => JSON.stringify(data);
