import { NextRequest, NextResponse } from 'next/server';
const backend = () => (process.env.BACKEND_URL || 'http://127.0.0.1:5000') + '/api/v1/';
const ACCESS = 'culinary_access';
const REFRESH = 'culinary_refresh';
const MAX_BODY = 6 * 1024 * 1024;
class BodyTooLarge extends Error {}
async function readBody(req: NextRequest): Promise<ArrayBuffer> {
  if (Number(req.headers.get('content-length') || 0) > MAX_BODY) throw new BodyTooLarge();
  const reader = req.body?.getReader();
  if (!reader) return new ArrayBuffer(0);
  const chunks: Uint8Array[] = []; let length = 0;
  try {
    while (true) {
      const { done, value } = await reader.read(); if (done) break;
      length += value.byteLength;
      if (length > MAX_BODY) { await reader.cancel(); throw new BodyTooLarge(); }
      chunks.push(value);
    }
  } finally { reader.releaseLock(); }
  const output = new Uint8Array(length); let offset = 0;
  for (const chunk of chunks) { output.set(chunk, offset); offset += chunk.byteLength; }
  return output.buffer;
}

type Session = { accessToken: string; refreshToken: string; expiresAt: string; user: unknown };
// Single flight prevents parallel browser requests from reusing a rotating token.
const rotations = new Map<string, Promise<Session | null>>();
async function rotate(token: string): Promise<Session | null> {
  const pending = rotations.get(token);
  if (pending) return pending;
  const promise = (async () => {
    const result = await fetch(backend() + 'auth/refresh', { method: 'POST', headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken: token }), cache: 'no-store', signal: AbortSignal.timeout(10000) });
    if (!result.ok) return null;
    const data = await result.json();
    return data.value as Session;
  })();
  rotations.set(token, promise);
  const cleanup = () => { setTimeout(() => { if (rotations.get(token) === promise) rotations.delete(token); }, 2000); };
  promise.then(cleanup, cleanup);
  return promise;
}
function cookies(response: NextResponse, session: Session | null, secure: boolean) {
  const common = { httpOnly: true, secure, sameSite: 'strict' as const, path: '/' };
  response.cookies.set(ACCESS, session?.accessToken || '', { ...common, maxAge: session ? 15 * 60 : 0 });
  response.cookies.set(REFRESH, session?.refreshToken || '', { ...common, maxAge: session ? 7 * 86400 : 0 });
}
async function handler(req: NextRequest, ctx: { params: Promise<{ path: string[] }> }) {
  const { path } = await ctx.params;
  if (!['auth', 'recipes', 'categories', 'files', 'jobs'].includes(path[0]) || path.some(s => s.includes('..') || s.includes('/') || s.includes('\\')))
    return NextResponse.json({ detail: 'Không tìm thấy tài nguyên.' }, { status: 404 });
  const mutating = !['GET', 'HEAD'].includes(req.method);
  if (mutating) {
    const origin = req.headers.get('origin');
    const expectedHost = req.headers.get('host') || req.nextUrl.host;
    let validOrigin = false;
    try { validOrigin = !!origin && new URL(origin).host === expectedHost; } catch {}
    if (!validOrigin)
      return NextResponse.json({ detail: 'Nguồn yêu cầu không hợp lệ.' }, { status: 403 });
  }
  const secure = req.nextUrl.protocol === 'https:' || req.headers.get('x-forwarded-proto') === 'https';
  let access = req.cookies.get(ACCESS)?.value;
  const refresh = req.cookies.get(REFRESH)?.value;
  let session: Session | null | undefined;
  const endpoint = path.map(encodeURIComponent).join('/');
  const isLogin = ['auth/login', 'auth/register', 'auth/google'].includes(endpoint);
  const isRefresh = ['auth/refresh', 'auth/refresh-token'].includes(endpoint);
  try {
    let body = mutating ? await readBody(req) : undefined;
    if (body && body.byteLength > 6 * 1024 * 1024) return NextResponse.json({ detail: 'Tệp quá lớn. Giới hạn ảnh là 5 MB.' }, { status: 413 });
    if (isRefresh) {
      session = refresh ? await rotate(refresh) : null;
      const out = NextResponse.json(session ? { user: session.user } : { detail: 'Phiên đăng nhập đã hết hạn.' }, { status: session ? 200 : 401 });
      cookies(out, session, secure); return out;
    }
    if (!access && refresh && !isLogin) { session = await rotate(refresh); access = session?.accessToken; }
    if (endpoint === 'auth/logout') body = new TextEncoder().encode(JSON.stringify({ refreshToken: session?.refreshToken || refresh || '' })).buffer;
    const send = () => fetch(backend() + endpoint + req.nextUrl.search, {
      method: req.method, body, cache: 'no-store', signal: AbortSignal.timeout(20000),
      headers: { ...(access ? { Authorization: `Bearer ${access}` } : {}),
        ...(req.headers.get('content-type') ? { 'Content-Type': req.headers.get('content-type')! } : {}),
        ...(req.headers.get('if-match') ? { 'If-Match': req.headers.get('if-match')! } : {}) },
    });
    let response = await send();
    if (response.status === 401 && refresh && session === undefined && !isLogin) {
      session = await rotate(refresh); access = session?.accessToken;
      if (session) {
        if (endpoint === 'auth/logout') body = new TextEncoder().encode(JSON.stringify({ refreshToken: session.refreshToken })).buffer;
        response = await send();
      }
    }
    let out: NextResponse;
    if (response.status === 204) out = new NextResponse(null, { status: 204 });
    else {
      const data = await response.json().catch(() => ({ detail: 'Máy chủ trả về dữ liệu không hợp lệ.' }));
      if (isLogin && response.ok) { session = data.value; out = NextResponse.json({ user: session!.user }, { status: response.status }); }
      else out = NextResponse.json(data, { status: response.status });
    }
    out.headers.set('Cache-Control', 'no-store');
    if (response.headers.has('retry-after')) out.headers.set('Retry-After', response.headers.get('retry-after')!);
    if (response.headers.has('etag')) out.headers.set('ETag', response.headers.get('etag')!);
    if (endpoint === 'auth/logout') cookies(out, null, secure);
    else if (session !== undefined) cookies(out, session, secure);
    return out;
  } catch (error) {
    if (error instanceof BodyTooLarge) return NextResponse.json({ detail: 'Tệp quá lớn. Giới hạn ảnh là 5 MB.' }, { status: 413 });
    return NextResponse.json({ detail: 'Máy chủ đang bận hoặc chưa sẵn sàng. Vui lòng thử lại sau.' }, { status: 503, headers: { 'Cache-Control': 'no-store' } });
  }
}
export const GET = handler;
export const POST = handler;
export const PUT = handler;
export const PATCH = handler;
export const DELETE = handler;
