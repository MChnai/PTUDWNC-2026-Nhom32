import AuthForm from '@/components/AuthForm';
export const metadata = { title: 'Đăng nhập', robots: { index: false, follow: false } };
export default async function Login({ searchParams }: { searchParams: Promise<{ next?: string }> }) { const { next } = await searchParams; return <AuthForm returnTo={next}/>; }
