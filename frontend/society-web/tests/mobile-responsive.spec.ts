import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Mobile Responsive', () => {

  test.use({ viewport: { width: 375, height: 812 } });

  test('should display bottom navigation on mobile', async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await expect(page.locator('nav')).toBeVisible();
  });

  test('should navigate using bottom nav', async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.getByRole('tab', { name: 'Flats' }).click();
    await expect(page).toHaveURL(/\/app\/flats/);
  });

  test('should open More menu on mobile', async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.getByRole('tab', { name: 'More' }).click();
    await expect(page.getByRole('menuitem', { name: 'Members' })).toBeVisible();
  });
});
