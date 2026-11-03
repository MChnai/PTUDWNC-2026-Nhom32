import { notFound } from 'next/navigation';
import { getRecipe, getRecipes, siteUrl } from '@/lib/server-api';
import RecipeExperience from '@/components/RecipeExperience';
import RecipeCard from '@/components/RecipeCard';
import ServiceNotice from '@/components/ServiceNotice';
export async function generateMetadata({ params }: { params: Promise<{ slug: string }> }) {
  const { slug } = await params; const r = await getRecipe(slug).catch(() => null); if (!r) return { title: 'Công thức', robots: { index: false } };
  const image = r.images.find(i => i.isPrimary)?.originalUrl || r.images[0]?.originalUrl;
  return { title: r.title, description: r.description?.slice(0, 160), alternates: { canonical: `/recipes/${r.slug}` }, openGraph: { title: r.title, description: r.description?.slice(0, 160), type: 'article', url: `/recipes/${r.slug}`, images: image ? [image] : undefined }, twitter: { card: 'summary_large_image' as const } };
}
export default async function Recipe({ params }: { params: Promise<{ slug: string }> }) {
  const { slug } = await params; let recipe;
  try { recipe = await getRecipe(slug); } catch (error) { if ((error as { status?: number }).status === 404) notFound(); return <div className="container page-section"><ServiceNotice/></div>; }
  const related = await getRecipes(`categoryId=${recipe.categoryId}&pageSize=4`).catch(() => null);
  const absolute = (url: string) => new URL(url, siteUrl()).toString();
  const schema = { '@context': 'https://schema.org', '@type': 'Recipe', name: recipe.title, description: recipe.description,
    image: recipe.images.map(i => absolute(i.originalUrl)), author: { '@type': 'Person', name: recipe.author?.displayName || 'Culinary Blog' },
    datePublished: recipe.publishedAt || recipe.createdAt, prepTime: `PT${recipe.prepTimeMinutes}M`, cookTime: `PT${recipe.cookTimeMinutes}M`, totalTime: `PT${recipe.prepTimeMinutes + recipe.cookTimeMinutes}M`, recipeYield: `${recipe.servings} khẩu phần`, recipeCategory: recipe.category?.name,
    recipeIngredient: recipe.ingredients.map(i => `${i.quantity} ${i.unit} ${i.name}`), recipeInstructions: recipe.steps.map(s => ({ '@type': 'HowToStep', name: s.title, text: s.description, image: s.imageUrl ? absolute(s.imageUrl) : undefined })),
    ...(recipe.nutrition ? { nutrition: { '@type': 'NutritionInformation', calories: `${recipe.nutrition.calories} calories`, proteinContent: `${recipe.nutrition.protein} g`, carbohydrateContent: `${recipe.nutrition.carbohydrates} g`, fatContent: `${recipe.nutrition.fat} g` } } : {}),
    ...(recipe.ratingCount > 0 ? { aggregateRating: { '@type': 'AggregateRating', ratingValue: recipe.ratingAverage, ratingCount: recipe.ratingCount } } : {}),
  };
  return <><script type="application/ld+json" dangerouslySetInnerHTML={{ __html: JSON.stringify(schema).replace(/</g, '\\u003c') }}/><RecipeExperience recipe={recipe}/>{related && related.items.some(r => r.id !== recipe.id) && <section className="container section related-section"><div className="section-heading"><div><span className="eyebrow">THÊM CHÚT CẢM HỨNG</span><h2>Có thể bạn cũng muốn thử</h2></div></div><div className="recipe-grid">{related.items.filter(r => r.id !== recipe.id).slice(0, 3).map(r => <RecipeCard recipe={r} key={r.id}/>)}</div></section>}</>;
}
