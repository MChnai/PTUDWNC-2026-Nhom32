#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."
if [ ! -f .env ]; then cp .env.example .env; fi
docker compose up --build -d
printf '%s\n' 'Website: http://localhost:3000' 'Email local: http://localhost:8025'
