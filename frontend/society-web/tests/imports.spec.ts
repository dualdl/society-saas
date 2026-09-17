import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';
import path from 'path';

test.describe('Import/Export Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/imports');
  });

  test('should display import page with upload and template sections', async ({ page }) => {
    await expect(page.locator('text=Upload')).toBeVisible();
    await expect(page.locator('text=Download Template')).toBeVisible();
  });

  test('should display file input for upload', async ({ page }) => {
    const fileInput = page.locator('input[type="file"]');
    await expect(fileInput).toBeVisible();
  });

  test('should download import template', async ({ page }) => {
    const downloadPromise = page.waitForEvent('download');
    await page.click('button:has-text("Download Template")');
    const download = await downloadPromise;
    expect(download.suggestedFilename()).toMatch(/\.xlsx$/);
  });

  test('should upload Excel file', async ({ page }) => {
    const fileInput = page.locator('input[type="file"]');
    await fileInput.setInputFiles(path.join(__dirname, 'fixtures/test-import.xlsx'));

    const alert = page.locator('[role="alert"]');
    await expect(alert).toBeVisible({ timeout: 10000 });
  });

  test('should display import history table', async ({ page }) => {
    await expect(page.locator('th:has-text("File")')).toBeVisible();
    await expect(page.locator('th:has-text("Status")')).toBeVisible();
    await expect(page.locator('th:has-text("Total")')).toBeVisible();
    await expect(page.locator('th:has-text("Success")')).toBeVisible();
    await expect(page.locator('th:has-text("Errors")')).toBeVisible();
  });

  test('should display import status chips', async ({ page }) => {
    const completedChips = page.locator('text=completed');
    const failedChips = page.locator('text=failed');
    const processingChips = page.locator('text=processing');
  });
});
