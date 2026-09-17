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

  test('should have link to society login', async ({ page }) => {
    await page.getByRole('link', { name: 'Society Login' }).click();
    await expect(page).toHaveURL(/\/society\/login/);
  });

  test('should have back to home link', async ({ page }) => {
    await page.getByRole('link', { name: 'Back to Home' }).click();
    await expect(page).toHaveURL(/\/$/);
  });
});
