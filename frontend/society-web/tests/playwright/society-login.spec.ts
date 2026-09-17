import { test, expect } from '@playwright/test';

test.describe('Society Login', () => {

  test.beforeEach(async ({ page }) => {
    await page.goto('/society/login');
  });

  test('should display login form', async ({ page }) => {
    await expect(page.locator('input[type="email"]')).toBeVisible();
    await expect(page.locator('input[type="password"]')).toBeVisible();
    await expect(page.locator('button[type="submit"]')).toBeVisible();
  });

  test('should show error for empty email', async ({ page }) => {
    await page.click('button[type="submit"]');
    const emailInput = page.locator('input[type="email"]');
    await expect(emailInput).toHaveAttribute('aria-invalid', 'true');
  });

  test('should show error for wrong credentials', async ({ page }) => {
    await page.fill('input[type="email"]', 'wrong@email.com');
    await page.fill('input[type="password"]', 'WrongPass123');
    await page.click('button[type="submit"]');
    const alert = page.locator('[role="alert"]');
    await expect(alert).toBeVisible({ timeout: 10000 });
  });

  test('should login successfully as society admin', async ({ page }) => {
    await page.fill('input[type="email"]', 'admin@sunshineresidency.com');
    await page.fill('input[type="password"]', 'Admin@123');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/app', { timeout: 15000 });
    await expect(page).toHaveURL(/\/app/);
  });

  test('should store token in localStorage after login', async ({ page }) => {
    await page.fill('input[type="email"]', 'admin@sunshineresidency.com');
    await page.fill('input[type="password"]', 'Admin@123');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/app', { timeout: 15000 });
    const token = await page.evaluate(() => localStorage.getItem('token'));
    expect(token).toBeTruthy();
  });

  test('should redirect to dashboard after login', async ({ page }) => {
    await page.fill('input[type="email"]', 'admin@sunshineresidency.com');
    await page.fill('input[type="password"]', 'Admin@123');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/app', { timeout: 15000 });
    await expect(page.locator('text=Dashboard')).toBeVisible();
  });
});
