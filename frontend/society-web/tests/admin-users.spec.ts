import { test, expect } from '@playwright/test';
import { loginAsSuperAdmin } from './helpers/auth';

test.describe('Super Admin - Users', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSuperAdmin(page);
    await page.goto('/admin/users');
    await page.waitForLoadState('networkidle');
  });

  test('should display users page', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
  });

  test('should display Add User button', async ({ page }) => {
    await expect(page.getByRole('button', { name: /Add User/ })).toBeVisible();
  });
});
