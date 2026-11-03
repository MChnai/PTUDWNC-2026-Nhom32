import { notFound } from 'next/navigation';
import Link from 'next/link';
import { ArrowLeft } from 'lucide-react';
import { getCategories, getRecipes } from '@/lib/server-api';
import RecipeCard from '@/components/RecipeCard';
import Pagination from '@/components/Pagination';
import ServiceNotice from '@/components/ServiceNotice';
export async function generateMetadata({ params }: { params: Promise<{ slug: string }> }) { const { slug } = await params; const cats = await getCategories().catch(() => []); const c = cats.find(x => x.slug === slug); return { title: c?.name || 'Danh mục', description: c?.description, alternates: { canonical: `/categories/${slug}` } }; }
export default async function Category({ params, searchParams }: { params: Promise<{ slug: string }>; searchParams: Promise<{ page?: string }> }) {
  const { slug } = await params; const { page } = await searchParams; const cats = await getCategories().catch(() => null);
  if (!cats) return <section className="container page-section"><ServiceNotice/></section>;
  const category = cats.find(c => c.slug === slug); if (!category) notFound();
  const data = await getRecipes(new URLSearchParams({ categoryId: category.id, page: String(Math.max(1, Number(page) || 1)), pageSize: '12' }).toString()).catch(() => null);
  return <section className="container page-section"><Link href="/categories" className="text-link back-link"><ArrowLeft size={16}/>Tất cả danh mục</Link><div className="page-intro"><span className="eyebrow">KHÁM PHÁ TỪNG HƯƠNG VỊ</span><h1>{category.name}<span className="accent-dot">.</span></h1><p>{category.description}</p></div>{!data ? <ServiceNotice/> : data.items.length ? <><div className="results-heading"><span>{data.totalCount} công thức trong danh mục</span></div><div className="recipe-grid">{data.items.map(r => <RecipeCard recipe={r} key={r.id}/>)}</div><Pagination page={data.pageNumber} total={data.totalPages} path={`/categories/${slug}`}/></> : <div className="empty-state"><h2>Hương vị mới đang được chuẩn bị</h2><p>Danh mục này chưa có công thức đã xuất bản.</p><Link href="/recipes" className="btn btn-dark">Xem các công thức khác</Link></div>}</section>;
}
