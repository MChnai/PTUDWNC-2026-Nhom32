import RecipeEditor from '@/components/RecipeEditor';
export default async function EditRecipe({ params }: { params: Promise<{ id: string }> }) { const { id } = await params; return <RecipeEditor id={id}/>; }
