import { test, expect } from '@playwright/test';
import { loginAsSuperAdmin } from './helpers/auth';

test.describe('Super Admin - Users', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSuperAdmin(page);
    await page.goto('/admin/users');
  });

  test('should display users table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.locator('th:has-text("Name")')).toBeVisible();
    await expect(page.locator('th:has-text("Email")')).toBeVisible();
    await expect(page.locator('th:has-text("Role")')).toBeVisible();
    await expect(page.locator('th:has-text("Status")')).toBeVisible();
  });

  test('should display Add User button', async ({ page }) => {
    await expect(page.locator('text=Add User')).toBeVisible();
  });

  test('should open Add User dialog', async ({ page }) => {
    await page.click('text=Add User');
    const dialog = page.locator('[role="dialog"], .MuiDialog-root');
    await expect(dialog).toBeVisible();
    await expect(dialog.locator('text=First Name')).toBeVisible();
    await expect(dialog.locator('text=Last Name')).toBeVisible();
    await expect(dialog.locator('text=Email')).toBeVisible();
    await expect(dialog.locator('text=Password')).toBeVisible();
  });

  test('should create a new user', async ({ page }) => {
    await page.click('text=Add User');
    const dialog = page.locator('[role="dialog"], .MuiDialog-root');

    await dialog.locator('input').nth(0).fill('Test');
    await dialog.locator('input').nth(1).fill('User');
    await dialog.locator('input[type="email"]').fill('testuser@example.com');
    await dialog.locator('input[type="password"]').fill('TestPass123');

    await dialog.locator('button:has-text("Create")').click();

    await expect(page.locator('text=Test User')).toBeVisible({ timeout: 5000 });
  });

  test('should toggle user status (block/enable)', async ({ page }) => {
    const toggleButton = page.locator('button:has-text("Block"), button:has-text("Enable")').first();
    if (await toggleButton.isVisible()) {
      await toggleButton.click();
      await page.waitForTimeout(500);
    }
  });

  test('should display role chips', async ({ page }) => {
    const superAdminChips = page.locator('text=Super Admin');
    const adminChips = page.locator('text=Admin');
    const total = await superAdminChips.count() + await adminChips.count();
    expect(total).toBeGreaterThan(0);
  });
});
