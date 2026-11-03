import { NextRequest, NextResponse } from 'next/server';
async function proxy(req: NextRequest, ctx: { params: Promise<{ path?: string[] }> }) {
  const access = req.cookies.get('culinary_access')?.value;
  if (!access) return NextResponse.redirect(new URL('/login?next=/hangfire', req.url));
  if (req.method === 'POST' && (!req.headers.get('origin') || new URL(req.headers.get('origin')!).host !== req.headers.get('host')))
    return new NextResponse('Nguồn yêu cầu không hợp lệ.', { status: 403 });
  const { path = [] } = await ctx.params;
  if (path.some(s => s.includes('..') || s.includes('/') || s.includes('\\'))) return new NextResponse(null, { status: 404 });
  try {
    const response = await fetch(`${process.env.BACKEND_URL || 'http://127.0.0.1:5000'}/hangfire${path.length ? '/' + path.map(encodeURIComponent).join('/') : ''}${req.nextUrl.search}`, {
      method: req.method, body: req.method === 'POST' ? await req.arrayBuffer() : undefined, cache: 'no-store',
      headers: { Authorization: `Bearer ${access}`, ...(req.headers.get('cookie') ? { Cookie: req.headers.get('cookie')! } : {}), ...(req.headers.get('content-type') ? { 'Content-Type': req.headers.get('content-type')! } : {}) },
      redirect: 'manual', signal: AbortSignal.timeout(20000),
    });
    const headers = new Headers(response.headers); headers.delete('content-encoding'); headers.delete('content-length'); headers.set('Cache-Control', 'no-store');
    if (response.status === 401) return NextResponse.redirect(new URL('/login?next=/hangfire', req.url));
    return new NextResponse(await response.arrayBuffer(), { status: response.status, headers });
  } catch { return new NextResponse('Tác vụ nền chưa sẵn sàng. Vui lòng thử lại sau.', { status: 503 }); }
}
export const GET = proxy;
export const POST = proxy;
