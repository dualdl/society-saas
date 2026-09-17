import { test, expect } from '@playwright/test';
import { loginAsSuperAdmin } from './helpers/auth';

test.describe('Super Admin - Societies', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSuperAdmin(page);
    await page.goto('/admin/societies');
  });

  test('should display societies table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.locator('th:has-text("Name")')).toBeVisible();
    await expect(page.locator('th:has-text("City")')).toBeVisible();
    await expect(page.locator('th:has-text("Email")')).toBeVisible();
    await expect(page.locator('th:has-text("Status")')).toBeVisible();
    await expect(page.locator('th:has-text("Created")')).toBeVisible();
  });

  test('should display Import JSON and Create Society buttons', async ({ page }) => {
    await expect(page.locator('text=Import JSON')).toBeVisible();
    await expect(page.locator('text=Create Society')).toBeVisible();
  });

  test('should display society status chips', async ({ page }) => {
    const activeChips = page.locator('text=Active');
    const inactiveChips = page.locator('text=Inactive');
    const total = await activeChips.count() + await inactiveChips.count();
    expect(total).toBeGreaterThan(0);
  });

  test('should show delete confirmation dialog', async ({ page }) => {
    const deleteButton = page.locator('[aria-label="Delete"]').first();
    if (await deleteButton.isVisible()) {
      await deleteButton.click();
      const dialog = page.locator('[role="dialog"], .MuiDialog-root');
      await expect(dialog).toBeVisible();
      await expect(dialog.locator('text=Are you sure')).toBeVisible();
      await dialog.locator('button:has-text("Cancel")').click();
    }
  });

  test('should have download backup button for each society', async ({ page }) => {
    const backupButtons = page.locator('[aria-label="Download Backup"], button:has-text("Backup")');
    expect(await backupButtons.count()).toBeGreaterThan(0);
  });
});
