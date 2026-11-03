import test from 'node:test';
import assert from 'node:assert/strict';
import { readFileSync, existsSync } from 'node:fs';
const root = new URL('../', import.meta.url);
const catalog = JSON.parse(readFileSync(new URL('backend/src/CulinaryBlog.API/Data/starter-catalog.json',root),'utf8'));
test('Starter catalog has valid references and complete cooking instructions', () => {
  const categories = new Set(catalog.categories.map(c=>c.id)); const authors = new Set(catalog.authors.map(a=>a.id));
  const slugs = new Set();
  for (const r of catalog.recipes) {
    assert.ok(categories.has(r.categoryId)); assert.ok(authors.has(r.authorId)); assert.ok(!slugs.has(r.slug)); slugs.add(r.slug);
    assert.ok(r.ingredients.length >= 5); assert.ok(r.steps.length >= 5); assert.ok(r.servings>0);
    for (const [i,s] of r.steps.entries()) { assert.equal(s.stepNumber,i+1); assert.ok(s.description.length>20); }
    for (const i of r.ingredients) { assert.ok(i.quantity>0); assert.ok(i.unit); }
    for (const i of r.images) assert.ok(existsSync(new URL('frontend/public'+i.originalUrl,root)));
  }
});
test('Culinary root is wired to the Next.js application', () => {
  const p=JSON.parse(readFileSync(new URL('package.json',root),'utf8'));
  assert.deepEqual(p.workspaces,['frontend']);
  const next=JSON.parse(readFileSync(new URL('frontend/package.json',root),'utf8'));
  assert.ok(next.dependencies.next); assert.ok(next.scripts.build.includes('next build'));
});
