import { test, expect } from '@playwright/test';

test.describe('Admin Login Page', () => {

  test.beforeEach(async ({ page }) => {
    await page.goto('/admin/login');
    await page.waitForLoadState('networkidle');
  });

  test('should display admin login form', async ({ page }) => {
    await expect(page.locator('h5:has-text("Super Admin Login")')).toBeVisible();
    await expect(page.locator('input[type="email"]')).toBeVisible();
    await expect(page.locator('input[type="password"]')).toBeVisible();
    await expect(page.locator('button[type="submit"]')).toBeVisible();
  });

  test('should show demo credentials', async ({ page }) => {
    await expect(page.locator('text=Demo Credentials')).toBeVisible();
    await expect(page.locator('text=superadmin@societypro.com')).toBeVisible();
  });

  test('should show platform-level admin access subtitle', async ({ page }) => {
    await expect(page.locator('text=Platform-level admin access')).toBeVisible();
  });

  test('should show error for non-super-admin credentials', async ({ page }) => {
    await page.fill('input[type="email"]', 'admin@sunshineresidency.com');
    await page.fill('input[type="password"]', 'Admin@123');
    await page.click('button[type="submit"]');
    await expect(page.locator('text=not a Super Admin')).toBeVisible({ timeout: 10000 });
  });

  test('should login successfully as super admin', async ({ page }) => {
    await page.fill('input[type="email"]', 'superadmin@societypro.com');
    await page.fill('input[type="password"]', 'SuperAdmin@123');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/admin/dashboard', { timeout: 30000 });
    await expect(page).toHaveURL(/\/admin\/dashboard/);
  });

  test('should store adminToken in localStorage', async ({ page }) => {
    await page.fill('input[type="email"]', 'superadmin@societypro.com');
    await page.fill('input[type="password"]', 'SuperAdmin@123');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/admin/dashboard', { timeout: 30000 });
    const adminToken = await page.evaluate(() => localStorage.getItem('adminToken'));
    const isAdmin = await page.evaluate(() => localStorage.getItem('isAdmin'));
    expect(adminToken).toBeTruthy();
    expect(isAdmin).toBe('true');
  });

  test('should have link to society login', async ({ page }) => {
    await page.getByRole('link', { name: 'Society Login' }).click();
    await expect(page).toHaveURL(/\/society\/login/);
  });

  test('should have back to home link', async ({ page }) => {
    await page.getByRole('link', { name: 'Back to Home' }).click();
    await expect(page).toHaveURL(/\/$/);
  });
});
