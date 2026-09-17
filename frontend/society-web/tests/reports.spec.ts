import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Reports Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/reports');
    await page.waitForLoadState('networkidle');
  });

  test('should display reports page with report cards', async ({ page }) => {
    await expect(page.locator('text=Flat Register')).toBeVisible();
    await expect(page.locator('text=Member Register')).toBeVisible();
    await expect(page.locator('text=Bill Register')).toBeVisible();
    await expect(page.locator('text=Payment Register')).toBeVisible();
    await expect(page.locator('text=Outstanding Report')).toBeVisible();
  });

  test('should have download buttons for reports', async ({ page }) => {
    const downloadButtons = page.getByRole('button', { name: /Download/ });
    expect(await downloadButtons.count()).toBeGreaterThan(0);
  });
});
