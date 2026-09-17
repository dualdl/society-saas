import { test, expect } from '@playwright/test';
import { loginAsSuperAdmin } from './helpers/auth';

test.describe('Super Admin Dashboard', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSuperAdmin(page);
  });

  test('should display dashboard with summary cards', async ({ page }) => {
    await expect(page.locator('text=Societies')).toBeVisible();
    await expect(page.locator('text=Flats')).toBeVisible();
    await expect(page.locator('text=Users')).toBeVisible();
    await expect(page.locator('text=Active')).toBeVisible();
  });

  test('should display Recent Societies section', async ({ page }) => {
    await expect(page.locator('text=Recent Societies')).toBeVisible();
  });
});
