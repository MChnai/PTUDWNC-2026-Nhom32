'use client';
import { useEffect, useState } from 'react';
import Link from 'next/link';
import { Bookmark, ArrowUpRight } from 'lucide-react';
import { readSaved } from '@/lib/saved';
import type { RecipeSummary } from '@/lib/types';
import RecipeCard from '@/components/RecipeCard';
export default function Saved() {
  const [recipes, setRecipes] = useState<RecipeSummary[]>([]); const [loading, setLoading] = useState(true);
  useEffect(() => { const update = () => { setRecipes(readSaved()); setLoading(false); }; update(); window.addEventListener('saved-change', update); return () => window.removeEventListener('saved-change', update); }, []);
  return <section className="container page-section"><div className="page-intro"><span className="eyebrow">NHỮNG MÓN NGON MUỐN NẤU LẠI</span><h1>Sổ tay <em>của riêng bạn.</em></h1><p>Công thức đã lưu trên trình duyệt và thiết bị này, sẵn sàng cho lần vào bếp tiếp theo.</p></div>{loading ? <div className="skeleton-grid" aria-label="Đang tải sổ tay"/> : recipes.length ? <div className="recipe-grid">{recipes.map(r => <RecipeCard key={r.id} recipe={r}/>)}</div> : <div className="empty-state"><Bookmark size={44}/><h2>Một cuốn sổ đang chờ được viết</h2><p>Nhấn biểu tượng lưu trên công thức bạn thích để giữ lại cảm hứng.</p><Link href="/recipes" className="btn btn-dark">Khám phá món ngon <ArrowUpRight size={17}/></Link></div>}</section>;
}
