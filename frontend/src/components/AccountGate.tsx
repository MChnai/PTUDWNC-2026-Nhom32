'use client';
import Link from 'next/link';
import { ChefHat, ArrowUpRight } from 'lucide-react';
export default function AccountGate({ next = '/studio' }: { next?: string }) { return <div className="empty-state account-gate"><ChefHat size={44}/><h1>Căn bếp riêng đang chờ bạn.</h1><p>Đăng nhập để viết công thức, quản lý bài chia sẻ và cập nhật hồ sơ của mình.</p><Link href={`/login?next=${encodeURIComponent(next)}`} className="btn btn-dark">Đăng nhập <ArrowUpRight size={17}/></Link></div>; }
