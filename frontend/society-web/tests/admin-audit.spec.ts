import { test, expect } from '@playwright/test';
import { loginAsSuperAdmin } from './helpers/auth';

test.describe('Super Admin - Audit', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSuperAdmin(page);
    await page.goto('/admin/audit');
    await page.waitForLoadState('networkidle');
  });

  test('should display audit trail page', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
  });

  test('should have filter options', async ({ page }) => {
    await expect(page.locator('text=Entity Type')).toBeVisible();
  });
});
