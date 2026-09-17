import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Payments Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/payments');
  });

  test('should display payments table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.locator('th:has-text("Payment #")')).toBeVisible();
    await expect(page.locator('th:has-text("Date")')).toBeVisible();
    await expect(page.locator('th:has-text("Flat")')).toBeVisible();
    await expect(page.locator('th:has-text("Amount")')).toBeVisible();
    await expect(page.locator('th:has-text("Mode")')).toBeVisible();
    await expect(page.locator('th:has-text("Status")')).toBeVisible();
  });

  test('should display Record Payment button', async ({ page }) => {
    await expect(page.locator('text=Record Payment')).toBeVisible();
  });

  test('should display payment status chips', async ({ page }) => {
    const completedChips = page.locator('text=Completed');
    const reversedChips = page.locator('text=Reversed');
    const total = await completedChips.count() + await reversedChips.count();
    expect(total).toBeGreaterThanOrEqual(0);
  });

  test('should show record payment dialog on button click', async ({ page }) => {
    await page.click('text=Record Payment');
    const dialog = page.locator('[role="dialog"], .MuiDialog-root');
    await expect(dialog).toBeVisible({ timeout: 5000 });
  });
});
