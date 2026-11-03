'use client';
import { useState, type FormEvent } from 'react';
import { Save, LoaderCircle } from 'lucide-react';
import { api, json } from '@/lib/api';
import { useAuth, useToast } from '@/components/Providers';
import AccountGate from '@/components/AccountGate';
export default function Profile() {
  const { user, loading, reload } = useAuth(); const toast = useToast(); const [busy, setBusy] = useState(false); const [error, setError] = useState('');
  if (loading) return <div className="container page-section"><div className="loading-state"><LoaderCircle className="spin"/>Đang mở hồ sơ...</div></div>;
  if (!user) return <div className="container page-section"><AccountGate next="/profile"/></div>;
  async function submit(e: FormEvent<HTMLFormElement>) { e.preventDefault(); setBusy(true); setError(''); const data = new FormData(e.currentTarget); try { await api('/auth/me', { method: 'PATCH', body: json(Object.fromEntries(data)) }); await reload(); toast('Đã cập nhật hồ sơ của bạn.'); } catch(e) { setError((e as Error).message); } finally { setBusy(false); } }
  return <section className="container page-section profile-section"><div className="page-intro"><span className="eyebrow">CÂU CHUYỆN CỦA BẠN</span><h1>Chào, <em>{user.fullName}.</em></h1><p>Một chút về người đứng sau những món ăn ngon.</p></div><div className="profile-card"><aside><span className="avatar avatar-large">{user.fullName.charAt(0)}{user.avatarUrl && <img src={user.avatarUrl} alt="" onError={e => { e.currentTarget.style.display = 'none'; }}/>}</span><h2>{user.fullName}</h2><span className="status-badge">{user.roles.includes('Admin') ? 'Quản trị viên' : 'Tác giả'}</span><p>{user.email}</p></aside><form onSubmit={submit} className="form-stack"><label>Tên hiển thị<input name="fullName" required minLength={2} maxLength={100} defaultValue={user.fullName}/></label><label>Đường dẫn ảnh đại diện<input name="avatarUrl" type="url" maxLength={500} defaultValue={user.avatarUrl || ''} placeholder="https://..."/><small>Ảnh công khai qua HTTPS hoặc HTTP. Để trống để bỏ ảnh.</small></label><label>Email<input type="email" value={user.email} readOnly/><small>Email được giữ cố định để bảo vệ tài khoản.</small></label><label>Tên đăng nhập<input value={user.userName} readOnly/></label>{error && <p role="alert" className="form-error">{error}</p>}<button className="btn btn-dark" disabled={busy}>{busy ? <LoaderCircle size={17} className="spin"/> : <Save size={17}/>} Lưu hồ sơ</button></form></div></section>;
}
