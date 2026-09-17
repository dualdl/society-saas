import { test, expect } from '@playwright/test';
import { loginAsSuperAdmin } from './helpers/auth';

test.describe('Super Admin - Audit', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSuperAdmin(page);
    await page.goto('/admin/audit');
  });

  test('should display audit filter', async ({ page }) => {
    await expect(page.locator('text=Entity Type')).toBeVisible();
    await expect(page.locator('text=From Date')).toBeVisible();
    await expect(page.locator('text=To Date')).toBeVisible();
  });

  test('should display entity type dropdown', async ({ page }) => {
    const dropdown = page.locator('select, [role="combobox"]').first();
    await dropdown.click();
    await expect(page.locator('text=All')).toBeVisible();
    await expect(page.locator('text=Tenant-Society')).toBeVisible();
    await expect(page.locator('text=User')).toBeVisible();
    await expect(page.locator('text=System')).toBeVisible();
  });

  test('should display audit table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.locator('th:has-text("Date/Time")')).toBeVisible();
    await expect(page.locator('th:has-text("Action")')).toBeVisible();
    await expect(page.locator('th:has-text("Entity")')).toBeVisible();
  });
});
