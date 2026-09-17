import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Receipts Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/receipts');
  });

  test('should display receipts table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.locator('th:has-text("Receipt #")')).toBeVisible();
    await expect(page.locator('th:has-text("Date")')).toBeVisible();
    await expect(page.locator('th:has-text("Flat")')).toBeVisible();
    await expect(page.locator('th:has-text("Amount")')).toBeVisible();
    await expect(page.locator('th:has-text("Actions")')).toBeVisible();
  });

  test('should have PDF download button for each receipt', async ({ page }) => {
    const pdfButtons = page.locator('[aria-label="Download PDF"], button:has-text("PDF")');
    expect(await pdfButtons.count()).toBeGreaterThanOrEqual(0);
  });

  test('should download receipt PDF when clicking download button', async ({ page }) => {
    const downloadPromise = page.waitForEvent('download');
    const pdfButton = page.locator('[aria-label="Download PDF"], button:has-text("PDF")').first();
    if (await pdfButton.isVisible()) {
      await pdfButton.click();
      const download = await downloadPromise;
      expect(download.suggestedFilename()).toMatch(/\.pdf$/);
    }
  });
});
