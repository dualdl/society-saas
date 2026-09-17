import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Settings Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/settings');
  });

  test('should display Society Information card', async ({ page }) => {
    await expect(page.locator('text=Society Information')).toBeVisible();
    await expect(page.locator('text=Society Name')).toBeVisible();
    await expect(page.locator('text=Email')).toBeVisible();
    await expect(page.locator('text=Phone')).toBeVisible();
    await expect(page.locator('text=Address')).toBeVisible();
    await expect(page.locator('text=City')).toBeVisible();
    await expect(page.locator('text=State')).toBeVisible();
    await expect(page.locator('text=Pincode')).toBeVisible();
  });

  test('should display Email Configuration card', async ({ page }) => {
    await expect(page.locator('text=Email Configuration')).toBeVisible();
    await expect(page.locator('text=Email Provider')).toBeVisible();
  });

  test('should display email provider dropdown', async ({ page }) => {
    const dropdown = page.locator('select, [role="combobox"]').filter({ hasText: /SMTP|Gmail|Outlook/ }).first();
    await expect(dropdown).toBeVisible();
  });

  test('should show SMTP fields when Custom SMTP is selected', async ({ page }) => {
    const dropdown = page.locator('select, [role="combobox"]').first();
    await dropdown.click();
    await page.click('text=Custom SMTP');
    await expect(page.locator('text=SMTP Host')).toBeVisible();
    await expect(page.locator('text=SMTP Port')).toBeVisible();
    await expect(page.locator('text=SMTP Username')).toBeVisible();
  });

  test('should show Gmail fields when Gmail is selected', async ({ page }) => {
    const dropdown = page.locator('select, [role="combobox"]').first();
    await dropdown.click();
    await page.click('text=Gmail');
    await expect(page.locator('text=Gmail Address')).toBeVisible();
    await expect(page.locator('text=Gmail App Password')).toBeVisible();
  });

  test('should update society information', async ({ page }) => {
    const nameInput = page.locator('input').first();
    await nameInput.clear();
    await nameInput.fill('Updated Society Name');

    await page.click('button:has-text("Save")');
    const alert = page.locator('[role="alert"]');
    await expect(alert).toBeVisible({ timeout: 5000 });
  });

  test('should have Save Settings button', async ({ page }) => {
    await expect(page.locator('button:has-text("Save")')).toBeVisible();
  });
});
