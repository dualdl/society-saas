import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Members Management', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/members');
    await page.waitForLoadState('networkidle');
  });

  test('should display members page', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
  });

  test('should display Add Member button', async ({ page }) => {
    await expect(page.getByRole('button', { name: /Add Member/ })).toBeVisible();
  });

  test('should have search input', async ({ page }) => {
    const searchInput = page.locator('input[placeholder*="Search"]');
    await expect(searchInput).toBeVisible();
  });
});
