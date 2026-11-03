import test from 'node:test';
import assert from 'node:assert/strict';
import { readFileSync, mkdirSync, writeFileSync, rmSync } from 'node:fs';
import ts from 'typescript';
import { NextRequest } from 'next/server.js';
const path = new URL('../frontend/.qa-tests/',import.meta.url);
mkdirSync(path,{recursive:true});
const source=readFileSync(new URL('../frontend/src/app/api/v1/[...path]/route.ts',import.meta.url),'utf8');
writeFileSync(new URL('route.mjs',path),ts.transpileModule(source,{compilerOptions:{module:ts.ModuleKind.ESNext,target:ts.ScriptTarget.ES2022}}).outputText.replaceAll("'next/server'","'next/server.js'"));
const route=await import(new URL('route.mjs',path));
const originalFetch=globalThis.fetch;
const session={accessToken:'secret-access',refreshToken:'secret-refresh',expiresAt:new Date(Date.now()+900000).toISOString(),user:{id:'u1',fullName:'Tester',roles:['Author']}};
function req(method,endpoint,options={}) { const headers={host:'localhost:3000',...(method==='GET'?{}:{origin:'http://localhost:3000','content-type':'application/json'}),...options.headers};return new NextRequest('http://localhost:3000/api/v1/'+endpoint,{method,headers,body:method==='GET'?undefined:options.body||'{}'}); }
const ctx=p=>({params:Promise.resolve({path:p.split('/')})});
try {
 await test('BFF login keeps both tokens out of JavaScript responses and sets HttpOnly cookies',async()=>{
  globalThis.fetch=async()=>Response.json({isSuccess:true,value:session});
  const result=await route.POST(req('POST','auth/login'),ctx('auth/login'));
  const text=await result.text();assert.equal(result.status,200);assert.ok(!text.includes('secret-'));assert.equal(JSON.parse(text).user.id,'u1');
  const cookie=result.headers.get('set-cookie');assert.ok(cookie.includes('HttpOnly'));assert.ok(cookie.includes('SameSite=strict'));assert.ok(cookie.includes('culinary_refresh='));
 });
 await test('BFF rejects cross-origin writes before forwarding',async()=>{
  let forwarded=false;globalThis.fetch=async()=>{forwarded=true;return Response.json({});};
  const result=await route.POST(req('POST','recipes',{headers:{origin:'https://attacker.example'}}),ctx('recipes'));
  assert.equal(result.status,403);assert.equal(forwarded,false);
 });
 await test('Malformed Origin is rejected with 403',async()=>{
  const result=await route.POST(req('POST','recipes',{headers:{origin:'invalid'}}),ctx('recipes'));assert.equal(result.status,403);
 });
 await test('BFF serializes concurrent refreshes for one rotating token',async()=>{
  let refreshes=0;globalThis.fetch=async(url,init)=>{
   if(String(url).endsWith('auth/refresh')){refreshes++;await new Promise(r=>setTimeout(r,25));return Response.json({isSuccess:true,value:session});}
   assert.equal(init.headers.Authorization,'Bearer secret-access');return Response.json({isSuccess:true,value:session.user});
  };
  const get=()=>route.GET(req('GET','auth/me',{headers:{cookie:'culinary_refresh=old-refresh'}}),ctx('auth/me'));
  const results=await Promise.all([get(),get()]);assert.equal(refreshes,1);assert.ok(results.every(r=>r.status===200));
 });
 await test('BFF preserves optimistic concurrency headers',async()=>{
  globalThis.fetch=async(url,init)=>{assert.equal(init.headers['If-Match'],'"123"');return Response.json({isSuccess:true,value:{rowVersion:124}},{headers:{ETag:'"124"'}});};
  const result=await route.PUT(req('PUT','recipes/id',{headers:{cookie:'culinary_access=a','if-match':'"123"'}}),ctx('recipes/id'));assert.equal(result.headers.get('etag'),'"124"');
 });
 await test('BFF refuses oversized bodies before contacting the API',async()=>{
  let forwarded=false;globalThis.fetch=async()=>{forwarded=true;return Response.json({});};
  const result=await route.POST(req('POST','recipes',{headers:{'content-length':String(7*1024*1024)}}),ctx('recipes'));assert.equal(result.status,413);assert.equal(forwarded,false);
 });
 await test('BFF maps upstream outages to a readable 503',async()=>{
  globalThis.fetch=async()=>{throw new Error('Network');};
  const result=await route.GET(req('GET','categories'),ctx('categories'));assert.equal(result.status,503);assert.ok((await result.json()).detail);
 });
 await test('BFF logout clears cookies and revokes only the server-held refresh token',async()=>{
  globalThis.fetch=async(url,init)=>{assert.equal(JSON.parse(Buffer.from(init.body).toString()).refreshToken,'owned-token');return new Response(null,{status:204});};
  const result=await route.POST(req('POST','auth/logout',{headers:{cookie:'culinary_access=a; culinary_refresh=owned-token'},body:JSON.stringify({refreshToken:'attacker-token'})}),ctx('auth/logout'));
  assert.equal(result.status,204);assert.ok(result.headers.get('set-cookie').includes('Max-Age=0'));
 });
} finally {globalThis.fetch=originalFetch;rmSync(path,{recursive:true,force:true});}
