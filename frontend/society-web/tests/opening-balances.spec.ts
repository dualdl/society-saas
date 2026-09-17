import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Opening Balances Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/opening-balances');
    await page.waitForLoadState('networkidle');
  });

  test('should display opening balances page', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
  });

  test('should have Save All button', async ({ page }) => {
    await expect(page.getByRole('button', { name: /Save All/ })).toBeVisible();
  });
});
