import Link from 'next/link';
import { ArrowUpRight } from 'lucide-react';
export default function NotFound() { return <section className="container notfound"><span>404</span><h1>Món này chưa có trong thực đơn.</h1><p>Công thức có thể đã được chuyển về bản nháp, lưu trữ hoặc không còn tồn tại.</p><Link href="/recipes" className="btn btn-dark">Tìm một món ngon khác <ArrowUpRight size={18}/></Link></section>; }
