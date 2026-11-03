#!/usr/bin/env python3
"""Run against a disposable local database/API. Uses only Python's standard library.
Creates test users and recipes; deletes its recipes at the end. Never run against production.
"""
import argparse, base64, json, os, time, uuid, urllib.request, urllib.error, urllib.parse
from pathlib import Path

parser=argparse.ArgumentParser()
parser.add_argument('--base',default='http://127.0.0.1:5000')
parser.add_argument('--admin-email',default=os.getenv('SEED_ADMIN_EMAIL','admin@culinary.local'))
parser.add_argument('--output',default='verification/api-results.json')
args=parser.parse_args()
base=args.base.rstrip('/')
opener=urllib.request.build_opener(urllib.request.ProxyHandler({}))
results=[]
def call(method,path,body=None,token=None,headers=None,raw=None):
    h={'Content-Type':'application/json',**(headers or {})}
    if token:h['Authorization']='Bearer '+token
    data=raw if raw is not None else json.dumps(body).encode() if body is not None else None
    req=urllib.request.Request(base+path,data=data,headers=h,method=method)
    try:r=opener.open(req,timeout=25)
    except urllib.error.HTTPError as e:r=e
    b=r.read()
    try: v=json.loads(b) if b else None
    except Exception:v=b.decode(errors='replace')
    return r.status,v,dict(r.headers)
def value(response):
    b=response[1]
    return b.get('value') if isinstance(b,dict) and b.get('isSuccess') is True else b
def expect(name,method,path,status,body=None,token=None,headers=None,raw=None,check=None):
    response=call(method,path,body,token,headers,raw)
    ok=response[0]==status and (check(value(response)) if check else True)
    results.append({'name':name,'passed':ok,'expectedStatus':status,'actualStatus':response[0]})
    print(('PASS' if ok else 'FAIL')+' '+name,flush=True)
    if not ok:raise AssertionError(name+' '+str(response[1])[:600])
    return response
prefix='/api/v1'
suffix=uuid.uuid4().hex[:10]
password='SmokeTest_Local_2026!'
created=[]
try:
    expect('Liveness', 'GET','/health/live',200,check=lambda x:x['status']=='Healthy')
    expect('Readiness', 'GET','/health/ready',200,check=lambda x:x['status']=='Healthy')
    cats=value(expect('Category list','GET',prefix+'/categories',200))
    assert cats
    cid=cats[0]['id']
    expect('Public list','GET',prefix+'/recipes?pageSize=3',200,check=lambda x:len(x['items'])<=3 and x['pageSize']==3)
    expect('Pagination clamps to 50','GET',prefix+'/recipes?pageSize=100',200,check=lambda x:x['pageSize']==50)
    expect('Invalid page rejected','GET',prefix+'/recipes?page=-1',422)
    expect('Invalid enum rejected','GET',prefix+'/recipes?difficulty=Banana',422)
    expect('Invalid sort rejected','GET',prefix+'/recipes?sort=unknown',422)
    expect('Search requires two characters','GET',prefix+'/recipes/search?q=a',422)
    expect('Empty search rejected','GET',prefix+'/recipes/search',422)
    expect('Unaccented search','GET',prefix+'/recipes/search?q=mi&sort=-rank',200,check=lambda x:any('Mì' in r['title'] for r in x['items']))
    expect('Filter difficulty/time/servings','GET',prefix+'/recipes?difficulty=Easy&maxTotalTime=30&minServings=2',200,check=lambda x:all(r['difficulty']==1 and r['totalTimeMinutes']<=30 and r['servings']>=2 for r in x['items']))
    expect('Anonymous write denied','POST',prefix+'/recipes',401,{})
    expect('Weak password rejected','POST',prefix+'/auth/register',422,{'fullName':'Smoke User','email':f'weak-{suffix}@example.test','userName':'weak_'+suffix,'password':'123'})
    def register(label):
        return value(expect('Register '+label,'POST',prefix+'/auth/register',201,{'fullName':'Smoke '+label,'email':f'{label}-{suffix}@example.test','userName':label+'_'+suffix,'password':password}))
    owner=register('owner');other=register('other');ot=owner['accessToken'];xt=other['accessToken']
    expect('Duplicate email is 409','POST',prefix+'/auth/register',409,{'fullName':'Smoke User','email':owner['user']['email'],'userName':'duplicate_'+suffix,'password':password})
    expect('Invalid login is 401','POST',prefix+'/auth/login',401,{'email':owner['user']['email'],'password':'Wrong123!'})
    expect('Profile is private','GET',prefix+'/auth/me',401)
    expect('Profile read','GET',prefix+'/auth/me',200,token=ot,check=lambda x:x['id']==owner['user']['id'])
    expect('Profile edit','PATCH',prefix+'/auth/me',200,{'fullName':'Smoke Updated','avatarUrl':''},ot,check=lambda x:x['fullName']=='Smoke Updated')
    expect('Unsafe avatar rejected','PATCH',prefix+'/auth/me',422,{'avatarUrl':'javascript:alert(1)'},ot)
    expect('Author cannot manage categories','POST',prefix+'/categories',403,{'name':'Smoke Cat '+suffix},ot)
    admin_password=os.getenv('SEED_ADMIN_PASSWORD')
    admin=None
    if admin_password:
        admin=value(expect('Admin login','POST',prefix+'/auth/login',200,{'email':args.admin_email,'password':admin_password}))
        at=admin['accessToken']
        cat_id=value(expect('Admin create category','POST',prefix+'/categories',201,{'name':'Kiểm thử '+suffix,'description':'Transient test'},at))
        expect('Admin update category','PUT',prefix+'/categories/'+cat_id,200,{'name':'Kiểm thử sửa '+suffix,'description':'Updated'},at)
        expect('Admin delete empty category','DELETE',prefix+'/categories/'+cat_id,200,token=at)
    payload={'title':'Công thức kiểm thử '+suffix,'description':'Atomic content test','instructions':'Extra instruction','notes':'Kitchen notes','prepTimeMinutes':10,'cookTimeMinutes':5,'servings':2,'difficulty':1,'categoryId':cid,'authorId':'00000000-0000-0000-0000-000000000000','nutrition':{'calories':123,'fat':4,'carbohydrates':15,'protein':6},'ingredients':[{'name':'Đậu hũ','quantity':200,'unit':'g','orderIndex':1,'notes':'Cắt nhỏ'}],'steps':[{'stepNumber':1,'title':'Chuẩn bị','description':'Chuẩn bị nguyên liệu sạch.','timerMinutes':1},{'stepNumber':2,'title':'Hoàn thiện','description':'Nấu chín và trình bày.'}]}
    rid=value(expect('Create draft with complete content','POST',prefix+'/recipes',201,payload,ot));created.append(rid);rp=prefix+'/recipes/'+rid
    original=value(expect('Owner reads draft','GET',rp,200,token=ot));version=str(original['rowVersion'])
    expect('Draft not public','GET',rp,404)
    expect('Draft ingredients not public','GET',rp+'/ingredients',404)
    expect('Draft steps not public','GET',rp+'/steps',404)
    expect('Other author cannot read draft','GET',rp,404,token=xt)
    expect('Management scoped to owner','GET',prefix+'/recipes/manage',200,token=ot,check=lambda x:any(r['id']==rid for r in x['items']))
    expect('Management hides foreign recipes','GET',prefix+'/recipes/manage',200,token=xt,check=lambda x:all(r['id']!=rid for r in x['items']))
    update={**payload,'title':'Nội dung mới '+suffix,'ingredients':original['ingredients'],'steps':original['steps'],'nutrition':{'calories':321,'fat':8,'carbohydrates':22,'protein':9}}
    update['ingredients'][0]['quantity']=400;update['steps'][0]['description']='Hướng dẫn đã được sửa.'
    expect('Foreign update forbidden','PUT',rp,403,update,xt,headers={'If-Match':version})
    expect('Foreign step deletion forbidden','DELETE',rp+'/steps/'+original['steps'][0]['id'],403,token=xt)
    expect('Missing If-Match rejected','PUT',rp,428,update,ot)
    expect('Foreign child IDs rejected atomically','PUT',rp,422,{**update,'ingredients':[{**original['ingredients'][0],'id':str(uuid.uuid4())}]},ot,headers={'If-Match':version})
    detail=value(expect('Save metadata/nutrition/steps/ingredients atomically','PUT',rp,200,update,ot,headers={'If-Match':version},check=lambda x:x['title']==update['title'] and x['ingredients'][0]['quantity']==400 and x['nutrition']['calories']==321 and x['steps'][0]['description']=='Hướng dẫn đã được sửa.'))
    expect('Stale update returns 409','PUT',rp,409,update,ot,headers={'If-Match':version})
    expect('Publish draft','PATCH',rp+'/publish',200,token=ot)
    expect('Publish idempotent','PATCH',rp+'/publish',200,token=ot)
    expect('Public slug route','GET',prefix+'/recipes/by-slug/'+detail['slug'],200)
    expect('Record published recipe view','POST',rp+'/view',204)
    expect('SRS direct slug route','GET',prefix+'/recipes/'+detail['slug'],200)
    expect('Public child read','GET',rp+'/steps',200,check=lambda x:len(x)==2)
    expect('Private category cannot be deleted when populated','DELETE',prefix+'/categories/'+cid,409,token=admin['accessToken']) if admin else None
    expect('Delete a step and renumber','DELETE',rp+'/steps/'+detail['steps'][0]['id'],204,token=ot)
    now=value(expect('Remaining step renumbered to 1','GET',rp,200,token=ot,check=lambda x:len(x['steps'])==1 and x['steps'][0]['stepNumber']==1))
    expect('Cannot delete final published step','DELETE',rp+'/steps/'+now['steps'][0]['id'],422,token=ot)
    # A valid tiny PNG; use multipart data and test extension/path sanitization.
    png=base64.b64decode('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+jZ9kAAAAASUVORK5CYII=')
    def upload(data,mime,name):
        boundary='CulinaryBoundary'+uuid.uuid4().hex
        raw=(f'--{boundary}\r\nContent-Disposition: form-data; name="file"; filename="{name}"\r\nContent-Type: {mime}\r\n\r\n'.encode()+data+f'\r\n--{boundary}\r\nContent-Disposition: form-data; name="isPrimary"\r\n\r\nfalse\r\n--{boundary}--\r\n'.encode())
        return raw,{'Content-Type':'multipart/form-data; boundary='+boundary}
    raw,headers=upload(b'<script>bad</script>','image/png','bad.png')
    expect('Forged image rejected','POST',rp+'/images',400,token=ot,raw=raw,headers=headers)
    raw,headers=upload(b'RIFF1234WAVEbad','image/webp','bad.webp')
    expect('RIFF audio cannot spoof WebP','POST',rp+'/images',400,token=ot,raw=raw,headers=headers)
    raw,headers=upload(png,'image/png','../../test.html')
    generic=value(expect('Generic image upload uses configured provider','POST',prefix+'/files/upload',200,token=ot,raw=raw,headers=headers))
    generic_path=prefix+'/files?fileUrl='+urllib.parse.quote(generic['url'],safe='')
    expect('Author cannot delete arbitrary generic media','DELETE',generic_path,403,token=ot)
    if admin:expect('Admin deletes generic media via same provider','DELETE',generic_path,200,token=admin['accessToken'])
    image=value(expect('Valid image upload sanitizes filename','POST',rp+'/images',201,token=ot,raw=raw,headers=headers,check=lambda x:x['isPrimary'] is True and '..' not in x['originalUrl'] and x['originalUrl'].endswith('.png')))
    expect('Set primary image','PATCH',rp+'/images/'+image['id']+'/primary',204,token=ot)
    expect('Delete image','DELETE',rp+'/images/'+image['id'],204,token=ot)
    expect('Archive hides public recipe','PATCH',rp+'/archive',204,token=ot)
    expect('Archived hidden from guest','GET',rp,404)
    expect('Unarchive returns draft','PATCH',rp+'/unarchive',204,token=ot)
    expect('Delete owned recipe','DELETE',rp,204,token=ot);created.remove(rid)
    expect('Deleted recipe absent','GET',rp,404,token=ot)
    empty_id=value(expect('Draft without steps allowed','POST',prefix+'/recipes',201,{**payload,'title':'Empty steps '+suffix,'steps':[]},ot));created.append(empty_id)
    expect('Cannot publish without steps','PATCH',prefix+'/recipes/'+empty_id+'/publish',422,token=ot)
    expect('Delete empty draft','DELETE',prefix+'/recipes/'+empty_id,204,token=ot);created.remove(empty_id)
    expect('Google configuration status','POST',prefix+'/auth/google',503,{'idToken':'invalid'})
    refreshed=value(expect('Refresh rotates token','POST',prefix+'/auth/refresh',200,{'refreshToken':owner['refreshToken']}))
    expect('Reuse old refresh token denied','POST',prefix+'/auth/refresh',401,{'refreshToken':owner['refreshToken']})
    expect('Reused family revoked','POST',prefix+'/auth/refresh',401,{'refreshToken':refreshed['refreshToken']})
    expect('Logout','POST',prefix+'/auth/logout',204,{'refreshToken':other['refreshToken']},xt)
    expect('Logged-out refresh denied','POST',prefix+'/auth/refresh',401,{'refreshToken':other['refreshToken']})
    expect('Author cannot access job dashboard','GET','/hangfire',403,token=xt)
    if admin:expect('Admin dashboard','GET','/hangfire',200,token=admin['accessToken'])
finally:
    for rid in created:
        try:call('DELETE',prefix+'/recipes/'+rid,token=ot)
        except Exception:pass
    out=Path(args.output);out.parent.mkdir(parents=True,exist_ok=True)
    out.write_text(json.dumps({'checks':len(results),'passed':sum(r['passed'] for r in results),'results':results},ensure_ascii=False,indent=2)+'\n')
print(f"{sum(r['passed'] for r in results)}/{len(results)} checks passed")
