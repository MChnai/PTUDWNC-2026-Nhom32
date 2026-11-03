import Link from 'next/link';
import { CookingPot, X } from 'lucide-react';
import type { Metadata } from 'next';
import { getRecipes, getCategories } from '@/lib/server-api';
import RecipeFilters from '@/components/RecipeFilters';
import RecipeCard from '@/components/RecipeCard';
import Pagination from '@/components/Pagination';
import ServiceNotice from '@/components/ServiceNotice';
export const metadata: Metadata = { title: 'Khám phá công thức', alternates: { canonical: '/recipes' } };
export default async function Recipes({ searchParams }: { searchParams: Promise<Record<string, string | string[] | undefined>> }) {
  const raw = await searchParams; const values: Record<string, string> = {};
  for (const key of ['q', 'categoryId', 'difficulty', 'maxTotalTime', 'minServings', 'sort', 'page', 'focus']) if (typeof raw[key] === 'string') values[key] = raw[key] as string;
  const q = new URLSearchParams(values); q.delete('focus'); q.set('pageSize', '12');
  const [data, cats] = await Promise.allSettled([getRecipes(q.toString()), getCategories()]);
  const recipes = data.status === 'fulfilled' ? data.value : null; const categories = cats.status === 'fulfilled' ? cats.value : [];
  const invalid = data.status === 'rejected' && data.reason?.status === 422;
  return <section className="container page-section"><div className="page-intro"><span className="eyebrow">MỘT CHÚT CẢM HỨNG CHO MỖI NGÀY</span><h1>Tìm món ngon, <em>theo cách của bạn.</em></h1><p>Từ bữa sáng gọn nhẹ đến bữa tối quây quần. Công thức phù hợp đang chờ bạn.</p></div><RecipeFilters key={q.toString()} categories={categories} values={values}/>
    <div className="results-heading"><span>{recipes ? <><strong>{recipes.totalCount}</strong> công thức{values.q ? ` cho “${values.q}”` : ' để bạn khám phá'}</> : 'Kết quả tìm kiếm'}</span>{Object.keys(values).some(k => !['sort', 'page', 'focus'].includes(k)) && <Link href="/recipes" className="clear-filter">Xóa bộ lọc <X size={14}/></Link>}</div>
    {invalid ? <div className="empty-state"><SearchError/><h2>Bộ lọc chưa hợp lệ</h2><p>Từ khóa cần ít nhất 2 ký tự. Hãy kiểm tra lại các điều kiện tìm kiếm.</p><Link href="/recipes" className="btn btn-dark">Làm mới tìm kiếm</Link></div> : !recipes ? <ServiceNotice/> : recipes.items.length ? <><div className="recipe-grid">{recipes.items.map((r, i) => <RecipeCard key={r.id} recipe={r} index={i}/>)}</div><Pagination page={recipes.pageNumber} total={recipes.totalPages} path="/recipes" values={values}/></> : <div className="empty-state"><CookingPot size={46}/><h2>Chưa tìm thấy món phù hợp</h2><p>Thử từ khóa ngắn hơn hoặc bớt một điều kiện lọc nhé.</p><Link href="/recipes" className="btn btn-dark">Khám phá tất cả công thức</Link></div>}
  </section>;
}
function SearchError() { return <CookingPot size={40}/>; }
