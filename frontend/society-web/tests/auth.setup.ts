import { test as setup, expect } from '@playwright/test';
import fs from 'fs';
import path from 'path';

const BASE_URL = process.env.BASE_URL || 'https://society-saas-web.azurewebsites.net';
const SOCIETY_EMAIL = process.env.SOCIETY_EMAIL || 'admin@sunshineresidency.com';
const SOCIETY_PASSWORD = process.env.SOCIETY_PASSWORD || 'Admin@123';

const authDir = path.join(__dirname, '.auth');

// Ensure auth directory exists
if (!fs.existsSync(authDir)) {
  fs.mkdirSync(authDir, { recursive: true });
}

setup('authenticate as society admin', async ({ page }) => {
  await page.goto(`${BASE_URL}/society/login`);
  await page.waitForLoadState('networkidle');
  await page.fill('input[type="email"]', SOCIETY_EMAIL);
  await page.fill('input[type="password"]', SOCIETY_PASSWORD);
  await page.click('button[type="submit"]');
  await page.waitForURL('**/app', { timeout: 15000 });

  // Save storage state
  await page.context().storageState({
    path: path.join(authDir, 'society-admin.json'),
  });
});
