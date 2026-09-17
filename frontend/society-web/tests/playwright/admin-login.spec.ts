import { test, expect } from '@playwright/test';

test.describe('Admin Login', () => {

  test.beforeEach(async ({ page }) => {
    await page.goto('/admin/login');
  });

  test('should display admin login form', async ({ page }) => {
    await expect(page.locator('input[type="email"]')).toBeVisible();
    await expect(page.locator('input[type="password"]')).toBeVisible();
    await expect(page.locator('button[type="submit"]')).toBeVisible();
  });

  test('should show error for non-super-admin credentials', async ({ page }) => {
    await page.fill('input[type="email"]', 'admin@sunshineresidency.com');
    await page.fill('input[type="password"]', 'Admin@123');
    await page.click('button[type="submit"]');
    const alert = page.locator('[role="alert"]');
    await expect(alert).toBeVisible({ timeout: 10000 });
  });

  test('should login successfully as super admin', async ({ page }) => {
    await page.fill('input[type="email"]', 'superadmin@societypro.com');
    await page.fill('input[type="password"]', 'SuperAdmin@123');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/admin/dashboard', { timeout: 15000 });
    await expect(page).toHaveURL(/\/admin\/dashboard/);
  });

  test('should store adminToken in localStorage', async ({ page }) => {
    await page.fill('input[type="email"]', 'superadmin@societypro.com');
    await page.fill('input[type="password"]', 'SuperAdmin@123');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/admin/dashboard', { timeout: 15000 });
    const adminToken = await page.evaluate(() => localStorage.getItem('adminToken'));
    const isAdmin = await page.evaluate(() => localStorage.getItem('isAdmin'));
    expect(adminToken).toBeTruthy();
    expect(isAdmin).toBe('true');
  });
});
