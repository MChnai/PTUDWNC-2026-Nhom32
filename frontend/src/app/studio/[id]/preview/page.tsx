import PrivateRecipe from '@/components/PrivateRecipe';
export default async function Preview({ params }: { params: Promise<{ id: string }> }) { const { id } = await params; return <PrivateRecipe id={id}/>; }
