import { test, expect } from '@playwright/test';

test.describe('Society Login Page', () => {

  test.beforeEach(async ({ page }) => {
    await page.goto('/society/login');
    await page.waitForLoadState('networkidle');
  });

  test('should display login form', async ({ page }) => {
    await expect(page.locator('h5:has-text("Society Login")')).toBeVisible();
    await expect(page.locator('input[type="email"]')).toBeVisible();
    await expect(page.locator('input[type="password"]')).toBeVisible();
    await expect(page.locator('button[type="submit"]')).toBeVisible();
  });

  test('should show demo credentials', async ({ page }) => {
    await expect(page.locator('text=Demo Credentials')).toBeVisible();
    await expect(page.locator('text=admin@sunshineresidency.com')).toBeVisible();
  });

  test('should have link to admin login', async ({ page }) => {
    await page.getByRole('link', { name: 'Super Admin Login' }).click();
    await expect(page).toHaveURL(/\/admin\/login/);
  });

  test('should have back to home link', async ({ page }) => {
    await page.getByRole('link', { name: 'Back to Home' }).click();
    await expect(page).toHaveURL(/\/$/);
  });
});

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

  test('should have link to society login', async ({ page }) => {
    await page.getByRole('link', { name: 'Society Login' }).click();
    await expect(page).toHaveURL(/\/society\/login/);
  });

  test('should have back to home link', async ({ page }) => {
    await page.getByRole('link', { name: 'Back to Home' }).click();
    await expect(page).toHaveURL(/\/$/);
  });
});
