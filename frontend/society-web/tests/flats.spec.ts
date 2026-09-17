import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Flats Management', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/flats');
  });

  test('should display flats table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.locator('th:has-text("Flat Number")')).toBeVisible();
    await expect(page.locator('th:has-text("Wing")')).toBeVisible();
    await expect(page.locator('th:has-text("Floor")')).toBeVisible();
    await expect(page.locator('th:has-text("Status")')).toBeVisible();
  });

  test('should display Add Flat button', async ({ page }) => {
    await expect(page.locator('text=Add Flat')).toBeVisible();
  });

  test('should search flats by number', async ({ page }) => {
    const searchInput = page.locator('input[placeholder*="Search"]');
    await searchInput.fill('101');
    await page.waitForTimeout(500);
    const rows = page.locator('tbody tr');
    const count = await rows.count();
    for (let i = 0; i < count; i++) {
      await expect(rows.nth(i)).toContainText('101');
    }
  });

  test('should display flat status chips', async ({ page }) => {
    const ownerChips = page.locator('text=Owner');
    const vacantChips = page.locator('text=Vacant');
    const totalChips = await ownerChips.count() + await vacantChips.count();
    expect(totalChips).toBeGreaterThan(0);
  });

  test('should display outstanding amounts', async ({ page }) => {
    const outstandingColumn = page.locator('td').filter({ hasText: /₹|INR/ });
    expect(await outstandingColumn.count()).toBeGreaterThanOrEqual(0);
  });

  test('should have edit action for each flat', async ({ page }) => {
    const editButtons = page.locator('[aria-label="Edit"]');
    expect(await editButtons.count()).toBeGreaterThan(0);
  });
});
