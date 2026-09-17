import { test, expect } from '@playwright/test';

test.describe('Landing Page', () => {

  test('should display landing page with hero section', async ({ page }) => {
    await page.goto('/');
    await expect(page.getByRole('heading', { name: 'SocietyPro' }).first()).toBeVisible();
    await expect(page.getByRole('heading', { name: 'Smart Society Management' })).toBeVisible();
  });

  test('should display society search input', async ({ page }) => {
    await page.goto('/');
    await expect(page.locator('text=Find Your Society')).toBeVisible();
    await expect(page.locator('input[placeholder*="society"]')).toBeVisible();
  });

  test('should display login dropdown with two options', async ({ page }) => {
    await page.goto('/');
    await page.getByRole('button', { name: 'Login ▼' }).click();
    await expect(page.getByRole('menuitem', { name: 'Society Login' })).toBeVisible();
    await expect(page.getByRole('menuitem', { name: 'Super Admin Login' })).toBeVisible();
  });

  test('should navigate to society login from dropdown', async ({ page }) => {
    await page.goto('/');
    await page.getByRole('button', { name: 'Login ▼' }).click();
    await page.getByRole('menuitem', { name: 'Society Login' }).click();
    await expect(page).toHaveURL(/\/society\/login/);
  });

  test('should navigate to admin login from dropdown', async ({ page }) => {
    await page.goto('/');
    await page.getByRole('button', { name: 'Login ▼' }).click();
    await page.getByRole('menuitem', { name: 'Super Admin Login' }).click();
    await expect(page).toHaveURL(/\/admin\/login/);
  });

  test('should search for society by slug', async ({ page }) => {
    await page.goto('/');
    await page.fill('input[placeholder*="society"]', 'sunshineresidency');
    await page.getByRole('button', { name: 'Find' }).click();
    await expect(page).toHaveURL(/\/s\/sunshineresidency/);
  });

  test('should show error for invalid society slug', async ({ page }) => {
    await page.goto('/');
    await page.fill('input[placeholder*="society"]', 'nonexistent-society');
    await page.getByRole('button', { name: 'Find' }).click();
    await expect(page.locator('text=Society not found')).toBeVisible({ timeout: 10000 });
  });

  test('should have Features section', async ({ page }) => {
    await page.goto('/');
    await expect(page.locator('h4:has-text("Features")')).toBeVisible();
    await expect(page.locator('text=Smart Billing')).toBeVisible();
    await expect(page.getByRole('heading', { name: 'Payments' })).toBeVisible();
  });

  test('should have footer with copyright', async ({ page }) => {
    await page.goto('/');
    await expect(page.locator('text=© 2026 SocietyPro')).toBeVisible();
  });
});

test.describe('Society Landing Page', () => {

  test('should show society not found for invalid slug', async ({ page }) => {
    await page.goto('/s/invalid-society-xyz');
    await expect(page.locator('text=Society not found')).toBeVisible({ timeout: 10000 });
  });
});
