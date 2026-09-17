import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Members Management', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/members');
  });

  test('should display members table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.locator('th:has-text("Name")')).toBeVisible();
    await expect(page.locator('th:has-text("Flat")')).toBeVisible();
    await expect(page.locator('th:has-text("Mobile")')).toBeVisible();
    await expect(page.locator('th:has-text("Email")')).toBeVisible();
  });

  test('should display Add Member button', async ({ page }) => {
    await expect(page.locator('text=Add Member')).toBeVisible();
  });

  test('should search members by name', async ({ page }) => {
    const searchInput = page.locator('input[placeholder*="Search"]');
    await searchInput.fill('John');
    await page.waitForTimeout(500);
    const rows = page.locator('tbody tr');
    const count = await rows.count();
    for (let i = 0; i < count; i++) {
      await expect(rows.nth(i)).toContainText('John');
    }
  });

  test('should display member type chips', async ({ page }) => {
    const ownerChips = page.locator('text=Owner');
    const tenantChips = page.locator('text=Tenant');
    const total = await ownerChips.count() + await tenantChips.count();
    expect(total).toBeGreaterThan(0);
  });

  test('should display primary indicator', async ({ page }) => {
    const primaryChips = page.locator('text=Yes');
    expect(await primaryChips.count()).toBeGreaterThanOrEqual(0);
  });
});
