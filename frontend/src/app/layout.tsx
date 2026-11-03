import type { Metadata } from 'next';
import Header from '@/components/Header';
import Footer from '@/components/Footer';
import { Providers } from '@/components/Providers';
import { siteUrl } from '@/lib/server-api';
import './globals.css';
export const metadata: Metadata = {
  metadataBase: new URL(siteUrl()),
  title: { default: 'Culinary — Niềm vui trong gian bếp', template: '%s | Culinary Blog' },
  description: 'Khám phá những công thức dễ làm, học nấu ăn từng bước và tìm cảm hứng cho bữa ăn mỗi ngày. Cùng Culinary biến căn bếp thành nơi bạn yêu thích.',
  openGraph: { title: 'Culinary — Niềm vui trong gian bếp', locale: 'vi_VN', type: 'website', images: ['/images/hero.webp'] },
  twitter: { card: 'summary_large_image' },
};
export default function RootLayout({ children }: { children: React.ReactNode }) { return <html lang="vi"><body><Providers><a href="#main-content" className="skip-link">Đến nội dung chính</a><Header/><main id="main-content">{children}</main><Footer/></Providers></body></html>; }
