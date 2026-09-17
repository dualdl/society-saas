import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Billing Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/billing');
    await page.waitForLoadState('networkidle');
  });

  test('should display billing page', async ({ page }) => {
    await expect(page.locator('text=Generate Bills')).toBeVisible();
  });

  test('should display Generate Bills button', async ({ page }) => {
    await expect(page.getByRole('button', { name: /Generate Bills/ })).toBeVisible();
  });

  test('should display bills table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
  });
});
