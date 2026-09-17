import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Late Fee Configuration', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/late-fee');
    await page.waitForLoadState('networkidle');
  });

  test('should display late fee configuration page', async ({ page }) => {
    await expect(page.locator('text=Enable Late Fees')).toBeVisible();
  });

  test('should have Save button', async ({ page }) => {
    await expect(page.getByRole('button', { name: /Save/ })).toBeVisible();
  });
});
