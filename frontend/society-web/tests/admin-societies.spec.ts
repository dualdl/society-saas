import { test, expect } from '@playwright/test';
import { loginAsSuperAdmin } from './helpers/auth';

test.describe('Super Admin - Societies', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSuperAdmin(page);
    await page.goto('/admin/societies');
    await page.waitForLoadState('networkidle');
  });

  test('should display societies page', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
  });

  test('should display Create Society button', async ({ page }) => {
    await expect(page.getByRole('button', { name: /Create Society/ })).toBeVisible();
  });
});
