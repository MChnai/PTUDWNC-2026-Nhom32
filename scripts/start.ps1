$ErrorActionPreference = 'Stop'
Set-Location (Split-Path -Parent $PSScriptRoot)
if (-not (Test-Path '.env')) { Copy-Item '.env.example' '.env'; Write-Host 'Da tao .env cho moi truong local.' }
docker compose up --build -d
if ($LASTEXITCODE -ne 0) { throw 'Docker Compose khong khoi dong thanh cong. Kiem tra Docker Desktop va docker compose logs.' }
Write-Host 'Website: http://localhost:3000'
Write-Host 'Email local: http://localhost:8025'
Write-Host 'Quan tri MinIO: http://localhost:9001'
Write-Host 'Tai khoan quan tri local duoc cau hinh trong .env (SEED_ADMIN_EMAIL / SEED_ADMIN_PASSWORD).'
