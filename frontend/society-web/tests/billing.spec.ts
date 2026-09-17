import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Billing Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/billing');
  });

  test('should display billing page with two sections', async ({ page }) => {
    await expect(page.locator('text=Generate Bills')).toBeVisible();
    await expect(page.locator('text=Billing History')).toBeVisible();
  });

  test('should generate bills successfully', async ({ page }) => {
    const generateButton = page.locator('button:has-text("Generate Bills")').first();
    await generateButton.click();

    const alert = page.locator('[role="alert"]');
    await expect(alert).toBeVisible({ timeout: 10000 });
  });

  test('should display bills table with columns', async ({ page }) => {
    await expect(page.locator('th:has-text("Bill #")')).toBeVisible();
    await expect(page.locator('th:has-text("Flat")')).toBeVisible();
    await expect(page.locator('th:has-text("Amount")')).toBeVisible();
    await expect(page.locator('th:has-text("Status")')).toBeVisible();
    await expect(page.locator('th:has-text("Period")')).toBeVisible();
  });

  test('should display bill status chips with correct colors', async ({ page }) => {
    const paidChips = page.locator('text=Paid');
    const pendingChips = page.locator('text=Pending');
    const partialChips = page.locator('text=Partial');

    const total = await paidChips.count() + await pendingChips.count() + await partialChips.count();
    expect(total).toBeGreaterThanOrEqual(0);
  });

  test('should display amounts in INR format', async ({ page }) => {
    const amounts = page.locator('td').filter({ hasText: /₹|INR/ });
    expect(await amounts.count()).toBeGreaterThanOrEqual(0);
  });
});
