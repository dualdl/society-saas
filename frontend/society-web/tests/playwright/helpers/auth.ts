import { Page, expect } from '@playwright/test';

const SOCIETY_EMAIL = process.env.SOCIETY_EMAIL || 'admin@sunshineresidency.com';
const SOCIETY_PASSWORD = process.env.SOCIETY_PASSWORD || 'Admin@123';
const ADMIN_EMAIL = process.env.ADMIN_EMAIL || 'superadmin@societypro.com';
const ADMIN_PASSWORD = process.env.ADMIN_PASSWORD || 'SuperAdmin@123';

export async function loginAsSocietyAdmin(page: Page) {
  await page.goto('/society/login');
  await page.fill('input[type="email"]', SOCIETY_EMAIL);
  await page.fill('input[type="password"]', SOCIETY_PASSWORD);
  await page.click('button[type="submit"]');
  await page.waitForURL('**/app', { timeout: 15000 });
  await expect(page).toHaveURL(/\/app/);
}

export async function loginAsSuperAdmin(page: Page) {
  await page.goto('/admin/login');
  await page.fill('input[type="email"]', ADMIN_EMAIL);
  await page.fill('input[type="password"]', ADMIN_PASSWORD);
  await page.click('button[type="submit"]');
  await page.waitForURL('**/admin/dashboard', { timeout: 15000 });
  await expect(page).toHaveURL(/\/admin\/dashboard/);
}

export async function logout(page: Page) {
  const logoutButton = page.locator('text=Logout');
  if (await logoutButton.isVisible()) {
    await logoutButton.click();
  }
  await page.waitForURL('**/');
}

export async function waitForAlert(page: Page, type: 'success' | 'error' = 'success') {
  const alert = page.locator(`[role="alert"]`);
  await expect(alert).toBeVisible({ timeout: 15000 });
  return alert;
}
