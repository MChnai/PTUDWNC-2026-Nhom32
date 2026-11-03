import type { MetadataRoute } from 'next';
import { getCategories, getRecipes, siteUrl } from '@/lib/server-api';
export const dynamic = 'force-dynamic';
export default async function sitemap(): Promise<MetadataRoute.Sitemap> {
  const base = siteUrl(); const entries: MetadataRoute.Sitemap = ['/', '/recipes', '/categories', '/learn', '/about'].map(path => ({ url: base + path, changeFrequency: 'weekly', priority: path === '/' ? 1 : 0.8 }));
  const categories = await getCategories(); entries.push(...categories.map(c => ({ url: `${base}/categories/${c.slug}`, changeFrequency: 'weekly' as const, priority: 0.6 })));
  for (let page = 1; ; page++) {
    const data = await getRecipes(`page=${page}&pageSize=50`);
    entries.push(...data.items.map(r => ({ url: `${base}/recipes/${r.slug}`, lastModified: r.publishedAt || r.createdAt, changeFrequency: 'weekly' as const, priority: 0.7 })));
    if (page >= data.totalPages) break;
  }
  return entries;
}
