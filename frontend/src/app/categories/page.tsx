import Link from 'next/link';
import type { Metadata } from 'next';
import { ArrowUpRight, Leaf, Soup, Wheat, CookingPot, Salad, ChefHat } from 'lucide-react';
import { getCategories } from '@/lib/server-api';
import ServiceNotice from '@/components/ServiceNotice';
export const metadata: Metadata = { title: 'Danh mục món ăn', alternates: { canonical: '/categories' } };
export default async function Categories() {
  const categories = await getCategories().catch(() => null); const icons = [Soup, Leaf, Wheat, CookingPot, Salad, ChefHat];
  return <section className="container page-section"><div className="page-intro"><span className="eyebrow">MỖI KHẨU VỊ, MỘT THẾ GIỚI</span><h1>Bạn muốn nấu <em>điều gì hôm nay?</em></h1><p>Chọn một danh mục và để cảm hứng dẫn lối vào bếp.</p></div>{!categories ? <ServiceNotice/> : <div className="category-directory">{categories.map((c, i) => { const Icon = icons[i % icons.length]; return <Link href={`/categories/${c.slug}`} key={c.id} className={`directory-card category-${i % 6}`}><span className="category-icon"><Icon size={35} strokeWidth={1.3}/></span><span className="eyebrow">{c.recipeCount} CÔNG THỨC</span><h2>{c.name}</h2><p>{c.description || 'Những món ăn thân quen, dễ học và đầy cảm hứng.'}</p><span className="text-link">Vào bếp ngay <ArrowUpRight size={18}/></span></Link>; })}</div>}</section>;
}
