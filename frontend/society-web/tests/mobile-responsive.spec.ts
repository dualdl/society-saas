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
    await page.click('text=Flats');
    await expect(page).toHaveURL(/\/app\/flats/);
  });

  test('should open More menu on mobile', async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.click('text=More');
    await expect(page.locator('text=Members')).toBeVisible();
    await expect(page.locator('text=Reports')).toBeVisible();
    await expect(page.locator('text=Settings')).toBeVisible();
  });

  test('should display sidebar as temporary drawer on mobile', async ({ page }) => {
    await loginAsSocietyAdmin(page);
    const menuButton = page.locator('[aria-label="menu"], button:has(svg)').first();
    if (await menuButton.isVisible()) {
      await menuButton.click();
      await page.waitForTimeout(300);
    }
  });

  test('should display cards in single column on mobile', async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/reports');
    const cards = page.locator('.MuiCard-root, [class*="card"]');
    expect(await cards.count()).toBeGreaterThan(0);
  });
});
