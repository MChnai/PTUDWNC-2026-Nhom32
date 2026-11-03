import type { RecipeSummary } from './types';
const key = 'culinary.saved.v2';
export function readSaved(): RecipeSummary[] { try { const v = JSON.parse(localStorage.getItem(key) || '[]'); return Array.isArray(v) ? v.filter(r => r && typeof r.id === 'string' && typeof r.slug === 'string') : []; } catch { return []; } }
export function toggleSaved(recipe: RecipeSummary) {
  const current = readSaved(); const exists = current.some(r => r.id === recipe.id);
  const next = exists ? current.filter(r => r.id !== recipe.id) : [...current, recipe];
  localStorage.setItem(key, JSON.stringify(next)); window.dispatchEvent(new Event('saved-change')); return !exists;
}
