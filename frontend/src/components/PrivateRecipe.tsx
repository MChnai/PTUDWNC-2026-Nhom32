'use client';
import { useEffect, useState } from 'react';
import { api } from '@/lib/api';
import type { RecipeDetail } from '@/lib/types';
import { useAuth } from './Providers';
import AccountGate from './AccountGate';
import RecipeExperience from './RecipeExperience';
export default function PrivateRecipe({ id }: { id: string }) {
  const { user, loading } = useAuth(); const [recipe, setRecipe] = useState<RecipeDetail | null>(null); const [error, setError] = useState('');
  useEffect(() => { if (!user) return; let live = true; api<RecipeDetail>(`/recipes/${id}`).then(r => { if (live) setRecipe(r); }).catch(e => { if (live) setError(e.message); }); return () => { live = false; }; }, [id, user]);
  if (loading) return <div className="container page-section loading-state">Đang tải...</div>;
  if (!user) return <div className="container page-section"><AccountGate next={`/studio/${id}/preview`}/></div>;
  if (error) return <div className="container page-section"><p className="form-error" role="alert">{error}</p></div>;
  return recipe ? <RecipeExperience recipe={recipe} preview/> : <div className="container page-section loading-state">Đang mở công thức...</div>;
}
