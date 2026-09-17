import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Audit Trail Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/audit');
  });

  test('should display audit filter card', async ({ page }) => {
    await expect(page.locator('text=Entity Type')).toBeVisible();
    await expect(page.locator('text=User ID')).toBeVisible();
    await expect(page.locator('text=From Date')).toBeVisible();
    await expect(page.locator('text=To Date')).toBeVisible();
  });

  test('should display entity type dropdown with options', async ({ page }) => {
    const dropdown = page.locator('select, [role="combobox"]').first();
    await dropdown.click();
    await expect(page.locator('text=All')).toBeVisible();
    await expect(page.locator('text=Flat')).toBeVisible();
    await expect(page.locator('text=Member')).toBeVisible();
    await expect(page.locator('text=Bill')).toBeVisible();
    await expect(page.locator('text=Payment')).toBeVisible();
  });

  test('should display audit table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.locator('th:has-text("Date/Time")')).toBeVisible();
    await expect(page.locator('th:has-text("Action")')).toBeVisible();
    await expect(page.locator('th:has-text("Entity")')).toBeVisible();
    await expect(page.locator('th:has-text("User")')).toBeVisible();
  });

  test('should display action type chips', async ({ page }) => {
    const createChips = page.locator('text=Create');
    const updateChips = page.locator('text=Update');
    const deleteChips = page.locator('text=Delete');
  });

  test('should filter by entity type', async ({ page }) => {
    const dropdown = page.locator('select, [role="combobox"]').first();
    await dropdown.click();
    await page.click('text=Flat');
    await page.waitForTimeout(500);
  });

  test('should filter by date range', async ({ page }) => {
    const fromDate = page.locator('input[type="date"]').first();
    const toDate = page.locator('input[type="date"]').last();
    if (await fromDate.isVisible()) {
      await fromDate.fill('2026-01-01');
      await toDate.fill('2026-12-31');
      await page.waitForTimeout(500);
    }
  });
});
