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

  test('should login successfully with valid credentials', async ({ page }) => {
    await page.fill('input[type="email"]', 'admin@sunshineresidency.com');
    await page.fill('input[type="password"]', 'Admin@123');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/app', { timeout: 30000 });
    await expect(page).toHaveURL(/\/app/);
  });

  test('should store token in localStorage after login', async ({ page }) => {
    await page.fill('input[type="email"]', 'admin@sunshineresidency.com');
    await page.fill('input[type="password"]', 'Admin@123');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/app', { timeout: 30000 });
    const token = await page.evaluate(() => localStorage.getItem('token'));
    expect(token).toBeTruthy();
  });

  test('should redirect to dashboard after login', async ({ page }) => {
    await page.fill('input[type="email"]', 'admin@sunshineresidency.com');
    await page.fill('input[type="password"]', 'Admin@123');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/app', { timeout: 30000 });
    await expect(page.locator('text=Dashboard')).toBeVisible();
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
