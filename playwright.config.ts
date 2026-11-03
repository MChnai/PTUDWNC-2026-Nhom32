import { defineConfig } from '@playwright/test';
const baseURL = process.env.PLAYWRIGHT_BASE_URL || 'http://localhost:3000';
if (!['localhost', '127.0.0.1'].includes(new URL(baseURL).hostname)) throw new Error('E2E tests create disposable records and only support localhost.');
export default defineConfig({ testDir: './tests/e2e', fullyParallel: false, workers: 1, timeout: 45000,
  use: { baseURL, viewport: { width: 1365, height: 900 }, trace: 'retain-on-failure' }, reporter: 'list',
});
