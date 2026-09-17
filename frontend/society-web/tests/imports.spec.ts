import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Import/Export Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/imports');
    await page.waitForLoadState('networkidle');
  });

  test('should display import page', async ({ page }) => {
    await expect(page.locator('text=Upload')).toBeVisible();
  });

  test('should display file input for upload', async ({ page }) => {
    const fileInput = page.locator('input[type="file"]');
    await expect(fileInput).toBeVisible();
  });

  test('should have Download Template button', async ({ page }) => {
    await expect(page.getByRole('button', { name: /Download Template/ })).toBeVisible();
  });
});
