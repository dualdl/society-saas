import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Flats Management', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/flats');
    await page.waitForLoadState('networkidle');
  });

  test('should display flats page', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
  });

  test('should display Add Flat button', async ({ page }) => {
    await expect(page.getByRole('button', { name: /Add Flat/ })).toBeVisible();
  });

  test('should have search input', async ({ page }) => {
    const searchInput = page.locator('input[placeholder*="Search"]');
    await expect(searchInput).toBeVisible();
  });
});
