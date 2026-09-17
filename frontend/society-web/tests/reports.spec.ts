import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Reports Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/reports');
  });

  test('should display all 6 report cards', async ({ page }) => {
    await expect(page.locator('text=Flat Register')).toBeVisible();
    await expect(page.locator('text=Member Register')).toBeVisible();
    await expect(page.locator('text=Bill Register')).toBeVisible();
    await expect(page.locator('text=Payment Register')).toBeVisible();
    await expect(page.locator('text=Outstanding Report')).toBeVisible();
    await expect(page.locator('text=Audit Trail')).toBeVisible();
  });

  test('should download Flat Register Excel', async ({ page }) => {
    const downloadPromise = page.waitForEvent('download');
    await page.locator('text=Flat Register').locator('..').locator('button:has-text("Download")').click();
    const download = await downloadPromise;
    expect(download.suggestedFilename()).toMatch(/\.xlsx$/);
  });

  test('should download Member Register Excel', async ({ page }) => {
    const downloadPromise = page.waitForEvent('download');
    await page.locator('text=Member Register').locator('..').locator('button:has-text("Download")').click();
    const download = await downloadPromise;
    expect(download.suggestedFilename()).toMatch(/\.xlsx$/);
  });

  test('should download Bill Register Excel', async ({ page }) => {
    const downloadPromise = page.waitForEvent('download');
    await page.locator('text=Bill Register').locator('..').locator('button:has-text("Download")').click();
    const download = await downloadPromise;
    expect(download.suggestedFilename()).toMatch(/\.xlsx$/);
  });

  test('should download Payment Register Excel', async ({ page }) => {
    const downloadPromise = page.waitForEvent('download');
    await page.locator('text=Payment Register').locator('..').locator('button:has-text("Download")').click();
    const download = await downloadPromise;
    expect(download.suggestedFilename()).toMatch(/\.xlsx$/);
  });

  test('should download Outstanding Report Excel', async ({ page }) => {
    const downloadPromise = page.waitForEvent('download');
    await page.locator('text=Outstanding Report').locator('..').locator('button:has-text("Download")').click();
    const download = await downloadPromise;
    expect(download.suggestedFilename()).toMatch(/\.xlsx$/);
  });

  test('should navigate to Audit Trail from reports', async ({ page }) => {
    await page.locator('text=Audit Trail').locator('..').locator('button, a').first().click();
    await expect(page).toHaveURL(/\/app\/audit/);
  });
});
