'use client';
import Link from 'next/link';
import { Bookmark, Clock3, ArrowUpRight, Star } from 'lucide-react';
import { useEffect, useState } from 'react';
import type { RecipeSummary } from '@/lib/types';
import { useToast } from './Providers';
import { toggleSaved, readSaved } from '@/lib/saved';
export function FoodImage({ src, alt, className = '', eager = false }: { src?: string | null; alt: string; className?: string; eager?: boolean }) {
  const [failed, setFailed] = useState(false);
  useEffect(() => setFailed(false), [src]);
  return <img className={className} src={!src || failed ? '/images/placeholder.svg' : src} alt={alt} loading={eager ? 'eager' : 'lazy'} decoding="async" onError={() => setFailed(true)}/>;
}
export default function RecipeCard({ recipe, index = 0 }: { recipe: RecipeSummary; index?: number }) {
  const [saved, setSaved] = useState(false); const toast = useToast();
  useEffect(() => { const update = () => setSaved(readSaved().some(r => r.id === recipe.id)); update(); window.addEventListener('saved-change', update); return () => window.removeEventListener('saved-change', update); }, [recipe.id]);
  return <article className="recipe-card" style={{ animationDelay: `${Math.min(index, 8) * 45}ms` }}><div className="card-photo"><Link href={`/recipes/${recipe.slug}`} tabIndex={-1} aria-hidden="true"><FoodImage src={recipe.coverImage} alt={recipe.title}/></Link>
    <span className="difficulty">{['', 'Dễ làm', 'Vừa sức', 'Thử thách'][recipe.difficulty] || 'Công thức'}</span><button className={`save-btn ${saved ? 'is-saved' : ''}`} aria-label={saved ? `Bỏ lưu ${recipe.title}` : `Lưu ${recipe.title}`} aria-pressed={saved} onClick={() => { const result = toggleSaved(recipe); setSaved(result); toast(result ? 'Đã thêm vào sổ tay công thức trên thiết bị này.' : 'Đã bỏ lưu công thức.'); }}><Bookmark size={18} fill={saved ? 'currentColor' : 'none'}/></button></div>
    <div className="card-body"><span className="eyebrow card-category">{recipe.categoryName}</span><h3><Link href={`/recipes/${recipe.slug}`}>{recipe.title}</Link></h3><p>{recipe.summary}</p><div className="card-meta"><span><Clock3 size={14}/>{recipe.prepTimeMinutes + recipe.cookTimeMinutes} phút</span>{recipe.ratingCount > 0 && <span><Star size={13} className="star" fill="currentColor"/>{recipe.averageRating.toFixed(1)}</span>}<Link href={`/recipes/${recipe.slug}`} className="card-arrow" aria-label={`Xem ${recipe.title}`}><ArrowUpRight size={18}/></Link></div></div></article>;
}
