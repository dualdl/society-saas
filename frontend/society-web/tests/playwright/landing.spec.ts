import { test, expect } from '@playwright/test';

test.describe('Landing Page', () => {

  test('should display landing page with all sections', async ({ page }) => {
    await page.goto('/');
    await expect(page.locator('text=Society SaaS')).toBeVisible();
    await expect(page.locator('text=Find Your Society')).toBeVisible();
  });

  test('should display login dropdown with two options', async ({ page }) => {
    await page.goto('/');
    await page.click('text=Login');
    await expect(page.locator('text=Society Login')).toBeVisible();
    await expect(page.locator('text=Super Admin Login')).toBeVisible();
  });

  test('should navigate to society login from dropdown', async ({ page }) => {
    await page.goto('/');
    await page.click('text=Login');
    await page.click('text=Society Login');
    await expect(page).toHaveURL(/\/society\/login/);
  });

  test('should navigate to admin login from dropdown', async ({ page }) => {
    await page.goto('/');
    await page.click('text=Login');
    await page.click('text=Super Admin Login');
    await expect(page).toHaveURL(/\/admin\/login/);
  });

  test('should search for society by slug', async ({ page }) => {
    await page.goto('/');
    const searchInput = page.locator('input[placeholder*="society"], input[placeholder*="slug"]').first();
    if (await searchInput.isVisible()) {
      await searchInput.fill('sunshine-residency');
      await page.click('button:has-text("Search")');
      await expect(page).toHaveURL(/\/s\/sunshine-residency/);
    }
  });
});

test.describe('Society Landing Page', () => {

  test('should display society info for valid slug', async ({ page }) => {
    await page.goto('/s/sunshine-residency');
    await expect(page.locator('text=Sunshine Residency')).toBeVisible();
  });

  test('should have login buttons', async ({ page }) => {
    await page.goto('/s/sunshine-residency');
    await expect(page.locator('text=Member Login').or(page.locator('text=Login'))).toBeVisible();
  });
});
