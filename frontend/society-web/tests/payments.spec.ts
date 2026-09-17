import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Payments Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/payments');
    await page.waitForLoadState('networkidle');
  });

  test('should display payments page', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
  });

  test('should display Record Payment button', async ({ page }) => {
    await expect(page.getByRole('button', { name: /Record Payment/ })).toBeVisible();
  });
});
