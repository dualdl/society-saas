import { test as setup, expect } from '@playwright/test';
import fs from 'fs';
import path from 'path';

const authDir = path.join(__dirname, '.auth');

// Ensure auth directory exists
if (!fs.existsSync(authDir)) {
  fs.mkdirSync(authDir, { recursive: true });
}

setup('authenticate as society admin', async ({ page }) => {
  const email = process.env.SOCIETY_EMAIL || 'admin@sunshineresidency.com';
  const password = process.env.SOCIETY_PASSWORD || 'Admin@123';

  await page.goto('/society/login');
  await page.fill('input[type="email"]', email);
  await page.fill('input[type="password"]', password);
  await page.click('button[type="submit"]');
  await page.waitForURL('**/app', { timeout: 15000 });

  // Save storage state
  await page.context().storageState({
    path: path.join(authDir, 'society-admin.json'),
  });
});

setup('authenticate as super admin', async ({ page }) => {
  const email = process.env.ADMIN_EMAIL || 'superadmin@societypro.com';
  const password = process.env.ADMIN_PASSWORD || 'SuperAdmin@123';

  await page.goto('/admin/login');
  await page.fill('input[type="email"]', email);
  await page.fill('input[type="password"]', password);
  await page.click('button[type="submit"]');
  await page.waitForURL('**/admin/dashboard', { timeout: 15000 });

  // Save storage state
  await page.context().storageState({
    path: path.join(authDir, 'super-admin.json'),
  });
});
