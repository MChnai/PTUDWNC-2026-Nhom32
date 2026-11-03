'use client';
import Link from 'next/link';
import { usePathname, useRouter } from 'next/navigation';
import { useState } from 'react';
import { ChefHat, Search, Bookmark, Menu, X, ArrowUpRight, UserRound, LogOut } from 'lucide-react';
import { useAuth, useToast } from './Providers';
export function Brand() { return <Link href="/" className="brand" aria-label="Culinary Blog — trang chủ"><span className="brand-icon"><ChefHat size={25} strokeWidth={1.7}/></span><span>Culinary<span className="brand-dot">.</span><small>NIỀM VUI TRONG GIAN BẾP</small></span></Link>; }
export default function Header() {
  const pathname = usePathname(); const router = useRouter(); const { user, logout } = useAuth(); const toast = useToast();
  const [open, setOpen] = useState(false);
  const links = [['/', 'Trang chủ'], ['/recipes', 'Công thức'], ['/categories', 'Danh mục'], ['/learn', 'Học nấu ăn']];
  return <><div className="topbar"><span>Một chút cảm hứng. Một bữa ăn ngon.</span><span className="topbar-right">Nấu từ tâm, ngon mỗi ngày <span>✳</span></span></div>
    <header className="site-header"><div className="container header-inner"><Brand/>
      <nav className={open ? 'nav open' : 'nav'} aria-label="Điều hướng chính">
        {links.map(([href, label]) => <Link key={href} href={href} onClick={() => setOpen(false)} className={(href === '/' ? pathname === '/' : pathname.startsWith(href)) ? 'active' : ''}>{label}</Link>)}
        {!user && <Link className="mobile-auth-link" href="/login" onClick={() => setOpen(false)}>Đăng nhập</Link>}
        {user && <Link href="/studio" onClick={() => setOpen(false)} className={pathname.startsWith('/studio') ? 'active' : ''}>Góc tác giả</Link>}
      </nav>
      <div className="header-actions"><Link href="/recipes?focus=search" className="icon-btn" aria-label="Tìm công thức"><Search size={20}/></Link><Link href="/saved" className="icon-btn saved-nav" aria-label="Công thức đã lưu"><Bookmark size={20}/></Link>
        {user ? <div className="account-menu"><Link href="/profile" className="avatar" aria-label="Hồ sơ cá nhân">{user.fullName?.charAt(0) || <UserRound size={18}/>}{user.avatarUrl && <img src={user.avatarUrl} alt="" onError={e => { e.currentTarget.style.display = 'none'; }}/>}</Link><button className="icon-btn logout" aria-label="Đăng xuất" onClick={async () => { try { await logout(); router.push('/'); toast('Đã đăng xuất. Hẹn bạn ở bữa ăn tiếp theo!'); } catch(e) { toast((e as Error).message); } }}><LogOut size={18}/></button></div> : <Link href="/login" className="btn btn-small btn-dark login-btn">Vào bếp cùng nhau <ArrowUpRight size={15}/></Link>}
        <button className="icon-btn menu-btn" aria-label={open ? 'Đóng menu' : 'Mở menu'} aria-expanded={open} onClick={() => setOpen(!open)}>{open ? <X/> : <Menu/>}</button>
      </div></div></header></>;
}
