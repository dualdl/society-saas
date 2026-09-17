import { Page, expect } from '@playwright/test';

export async function loginAsSocietyAdmin(page: Page) {
  await page.goto('/society/login');
  await page.fill('input[type="email"]', process.env.SOCIETY_EMAIL || 'admin@sunshineresidency.com');
  await page.fill('input[type="password"]', process.env.SOCIETY_PASSWORD || 'Admin@123');
  await page.click('button[type="submit"]');
  await page.waitForURL('**/app');
  await expect(page).toHaveURL(/\/app/);
}

export async function loginAsSuperAdmin(page: Page) {
  await page.goto('/admin/login');
  await page.fill('input[type="email"]', process.env.ADMIN_EMAIL || 'superadmin@societypro.com');
  await page.fill('input[type="password"]', process.env.ADMIN_PASSWORD || 'SuperAdmin@123');
  await page.click('button[type="submit"]');
  await page.waitForURL('**/admin/dashboard');
  await expect(page).toHaveURL(/\/admin\/dashboard/);
}

export async function logout(page: Page) {
  const logoutButton = page.locator('text=Logout');
  if (await logoutButton.isVisible()) {
    await logoutButton.click();
  }
  await page.waitForURL('**/');
}
