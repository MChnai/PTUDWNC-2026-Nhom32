import AuthForm from '@/components/AuthForm';
export const metadata = { title: 'Tạo tài khoản', robots: { index: false, follow: false } };
export default async function Register({ searchParams }: { searchParams: Promise<{ next?: string }> }) { const { next } = await searchParams; return <AuthForm register returnTo={next}/>; }
