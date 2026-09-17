import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Receipts Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/receipts');
    await page.waitForLoadState('networkidle');
  });

  test('should display receipts page', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
  });
});
