'use client';
import { useRef, useState, type FormEvent } from 'react';
import Link from 'next/link';
import Script from 'next/script';
import { useRouter } from 'next/navigation';
import { ArrowUpRight, Eye, EyeOff, ChefHat, LoaderCircle } from 'lucide-react';
import { useAuth, useToast } from './Providers';
export default function AuthForm({ register = false, returnTo = '/studio' }: { register?: boolean; returnTo?: string }) {
  const { login } = useAuth(); const router = useRouter(); const toast = useToast(); const googleRef = useRef<HTMLDivElement>(null);
  const [busy, setBusy] = useState(false); const [error, setError] = useState(''); const [show, setShow] = useState(false);
  const safeTarget = returnTo.startsWith('/') && !returnTo.startsWith('//') && !returnTo.includes('\\') ? returnTo : '/studio';
  async function submit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault(); setBusy(true); setError(''); const data = new FormData(e.currentTarget);
    try { await login(register ? '/auth/register' : '/auth/login', Object.fromEntries(data)); toast(register ? 'Chào mừng bạn đến với căn bếp Culinary!' : 'Chào bạn, cùng vào bếp nhé!'); router.replace(safeTarget); router.refresh(); }
    catch(e) { setError((e as Error).message); } finally { setBusy(false); }
  }
  function renderGoogle() {
    type Google = { accounts: { id: { initialize: (args: { client_id: string; callback: (data: { credential: string }) => void }) => void; renderButton: (element: HTMLElement, args: object) => void } } };
    const google = (window as unknown as { google?: Google }).google;
    const clientId = process.env.NEXT_PUBLIC_GOOGLE_CLIENT_ID;
    if (!google || !googleRef.current || !clientId) return;
    google.accounts.id.initialize({ client_id: clientId, callback: async ({ credential }) => {
      setBusy(true); setError(''); try { await login('/auth/google', { idToken: credential }); router.replace(safeTarget); router.refresh(); } catch(e) { setError((e as Error).message); } finally { setBusy(false); }
    } });
    google.accounts.id.renderButton(googleRef.current, { theme: 'outline', size: 'large', width: 340, text: 'continue_with', locale: 'vi' });
  }
  return <section className="container auth-section"><div className="auth-story"><div className="auth-story-photo"><img src="/images/salad-ngu-coc.webp" alt="Bữa ăn tươi ngon tự tay chuẩn bị"/></div><div className="auth-story-copy"><span className="eyebrow">CÙNG NHAU VÀO BẾP</span><h2>Một bữa ăn ngon.<br/><em>Một ngày thật khác.</em></h2><p>Lưu cảm hứng, chia sẻ công thức và tìm niềm vui từ những điều giản dị.</p></div></div><div className="auth-form-wrap"><span className="auth-symbol"><ChefHat size={28}/></span><span className="eyebrow">CĂN BẾP LUÔN CHÀO ĐÓN BẠN</span><h1>{register ? 'Bắt đầu hành trình mới.' : 'Mừng bạn trở lại.'}</h1><p>{register ? 'Tạo tài khoản để chia sẻ món ngon của riêng mình.' : 'Đăng nhập để tiếp tục câu chuyện trong gian bếp.'}</p>
    <form className="form-stack" onSubmit={submit}>{register && <><label>Tên hiển thị<input name="fullName" autoComplete="name" required minLength={2} maxLength={100} placeholder="Tên mà mọi người sẽ thấy"/></label><label>Tên đăng nhập<input name="userName" autoComplete="username" required minLength={3} maxLength={50} pattern="[a-zA-Z0-9_]+" title="Từ 3–50 chữ cái không dấu, số hoặc dấu gạch dưới" placeholder="vi_du: bep_cua_y"/></label></>}<label>Email<input type="email" name="email" autoComplete="email" required maxLength={150} placeholder="ban@example.com"/></label><label>Mật khẩu<div className="password-field"><input type={show ? 'text' : 'password'} name="password" aria-label="Mật khẩu" autoComplete={register ? 'new-password' : 'current-password'} required minLength={register ? 8 : 1} maxLength={128} placeholder={register ? 'Tạo mật khẩu an toàn' : 'Nhập mật khẩu của bạn'}/><button type="button" className="icon-btn" aria-label={show ? 'Ẩn mật khẩu' : 'Hiện mật khẩu'} onClick={() => setShow(!show)}>{show ? <EyeOff size={18}/> : <Eye size={18}/>}</button></div>{register && <small>Ít nhất 8 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt.</small>}</label>{error && <div className="form-error" role="alert">{error}</div>}<button className="btn btn-dark btn-full" disabled={busy}>{busy ? <LoaderCircle className="spin" size={18}/> : <>{register ? 'Tạo tài khoản' : 'Đăng nhập'}<ArrowUpRight size={18}/></>}</button></form>
    {process.env.NEXT_PUBLIC_GOOGLE_CLIENT_ID && <><div className="form-divider">hoặc</div><div ref={googleRef} className="google-button"/><Script src="https://accounts.google.com/gsi/client" onReady={renderGoogle} onError={() => setError('Chưa tải được đăng nhập Google. Bạn có thể tiếp tục bằng email.')}/></>}
    <p className="auth-switch">{register ? 'Đã có tài khoản?' : 'Bạn mới đến Culinary?'} <Link href={`${register ? '/login' : '/register'}?next=${encodeURIComponent(safeTarget)}`}>{register ? 'Đăng nhập' : 'Tạo tài khoản'}</Link></p><Link href="/recipes" className="continue-guest">Tiếp tục khám phá công thức <ArrowUpRight size={14}/></Link></div></section>;
}
