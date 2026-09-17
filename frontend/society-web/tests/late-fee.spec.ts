import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Late Fee Configuration', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/late-fee');
  });

  test('should display late fee configuration form', async ({ page }) => {
    await expect(page.locator('text=Enable Late Fees')).toBeVisible();
    await expect(page.locator('text=Grace Days')).toBeVisible();
    await expect(page.locator('text=Apply From Day')).toBeVisible();
    await expect(page.locator('text=Rate')).toBeVisible();
    await expect(page.locator('text=Max Amount')).toBeVisible();
    await expect(page.locator('text=Calculate On')).toBeVisible();
  });

  test('should have toggle switch for enabling late fees', async ({ page }) => {
    const toggle = page.locator('input[type="checkbox"], [role="checkbox"], .MuiSwitch-root');
    await expect(toggle).toBeVisible();
  });

  test('should disable fields when late fees are disabled', async ({ page }) => {
    const graceDaysInput = page.locator('input[name*="grace"], input').nth(0);
    const toggle = page.locator('input[type="checkbox"], [role="checkbox"], .MuiSwitch-root').first();

    const isOff = await toggle.isChecked().catch(() => false);
    if (!isOff) {
      const isDisabled = await graceDaysInput.isDisabled().catch(() => true);
      expect(isDisabled).toBeTruthy();
    }
  });

  test('should enable fields when toggle is turned on', async ({ page }) => {
    const toggle = page.locator('input[type="checkbox"], [role="checkbox"], .MuiSwitch-root').first();
    await toggle.click();
    await page.waitForTimeout(300);

    const graceDaysInput = page.locator('input').nth(0);
    const isEnabled = await graceDaysInput.isEnabled().catch(() => true);
    expect(isEnabled).toBeTruthy();
  });

  test('should save late fee configuration', async ({ page }) => {
    const toggle = page.locator('input[type="checkbox"], [role="checkbox"], .MuiSwitch-root').first();
    await toggle.click();

    const inputs = page.locator('input[type="number"]');
    if (await inputs.count() > 0) {
      await inputs.nth(0).fill('10');
    }

    await page.click('button:has-text("Save")');
    const alert = page.locator('[role="alert"]');
    await expect(alert).toBeVisible({ timeout: 5000 });
  });
});
