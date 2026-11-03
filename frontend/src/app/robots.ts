import type { MetadataRoute } from 'next';
import { siteUrl } from '@/lib/server-api';
export default function robots(): MetadataRoute.Robots { return { rules: { userAgent: '*', allow: '/', disallow: ['/hangfire', '/studio', '/profile', '/api/', '/login', '/register', '/saved'] }, sitemap: `${siteUrl()}/sitemap.xml` }; }
