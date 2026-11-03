import Link from 'next/link';
import { ChevronLeft, ChevronRight } from 'lucide-react';
export default function Pagination({ page, total, path, values = {} }: { page: number; total: number; path: string; values?: Record<string, string> }) {
  if (total <= 1) return null;
  const link = (n: number) => { const q = new URLSearchParams(values); q.set('page', String(n)); q.delete('focus'); return `${path}?${q}`; };
  const pages = Array.from(new Set([1, ...Array.from({ length: 5 }, (_, i) => page - 2 + i).filter(n => n > 1 && n < total), total])).sort((a, b) => a - b);
  return <nav className="pagination" aria-label="Phân trang công thức">{page > 1 ? <Link href={link(page - 1)} aria-label="Trang trước"><ChevronLeft size={17}/></Link> : <span className="disabled"><ChevronLeft size={17}/></span>}{pages.map((n, i) => <span className="pagination-part" key={n}>{i > 0 && n > pages[i - 1] + 1 && <span className="ellipsis">…</span>}<Link href={link(n)} aria-current={n === page ? 'page' : undefined} className={n === page ? 'active' : ''}>{n}</Link></span>)}{page < total ? <Link href={link(page + 1)} aria-label="Trang tiếp theo"><ChevronRight size={17}/></Link> : <span className="disabled"><ChevronRight size={17}/></span>}</nav>;
}
