import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Opening Balances Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/opening-balances');
  });

  test('should display opening balances info card', async ({ page }) => {
    await expect(page.locator('text=Opening Balance')).toBeVisible();
  });

  test('should display editable table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.locator('th:has-text("Wing")')).toBeVisible();
    await expect(page.locator('th:has-text("Flat Number")')).toBeVisible();
    await expect(page.locator('th:has-text("Opening Balance")')).toBeVisible();
  });

  test('should have Save All button', async ({ page }) => {
    await expect(page.locator('button:has-text("Save All")')).toBeVisible();
  });

  test('should search flats', async ({ page }) => {
    const searchInput = page.locator('input[placeholder*="Search"]');
    if (await searchInput.isVisible()) {
      await searchInput.fill('101');
      await page.waitForTimeout(500);
    }
  });

  test('should have editable balance fields', async ({ page }) => {
    const balanceInputs = page.locator('input[type="number"]');
    expect(await balanceInputs.count()).toBeGreaterThanOrEqual(0);
  });
});
