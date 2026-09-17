import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Audit Trail Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/audit');
    await page.waitForLoadState('networkidle');
  });

  test('should display audit trail page', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
  });

  test('should have filter options', async ({ page }) => {
    await expect(page.locator('text=Entity Type')).toBeVisible();
  });
});
