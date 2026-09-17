import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Settings Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/settings');
    await page.waitForLoadState('networkidle');
  });

  test('should display settings page', async ({ page }) => {
    await expect(page.locator('text=Society Information')).toBeVisible();
  });

  test('should have Save button', async ({ page }) => {
    await expect(page.getByRole('button', { name: /Save/ })).toBeVisible();
  });
});
