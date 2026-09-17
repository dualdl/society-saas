import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Charges Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/charges');
    await page.waitForLoadState('networkidle');
  });

  test('should display charges page', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
  });

  test('should display Add Charge button', async ({ page }) => {
    await expect(page.getByRole('button', { name: /Add Charge/ })).toBeVisible();
  });
});
