import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Society Dashboard', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
  });

  test('should display dashboard with summary cards', async ({ page }) => {
    await expect(page.locator('text=Total Flats')).toBeVisible();
    await expect(page.locator('text=Total Members')).toBeVisible();
    await expect(page.locator('text=Collection')).toBeVisible();
    await expect(page.locator('text=Outstanding')).toBeVisible();
  });

  test('should display numeric values in summary cards', async ({ page }) => {
    const flatsCard = page.locator('text=Total Flats').locator('..');
    await expect(flatsCard.locator('[class*="value"], [class*="number"], h2, h3')).toBeVisible();
  });

  test('should display Quick Actions section', async ({ page }) => {
    await expect(page.locator('text=Generate Bills')).toBeVisible();
    await expect(page.locator('text=Record Payment')).toBeVisible();
    await expect(page.locator('text=Import Excel')).toBeVisible();
    await expect(page.locator('text=Reports')).toBeVisible();
  });

  test('should navigate to billing from Quick Actions', async ({ page }) => {
    await page.click('text=Generate Bills');
    await expect(page).toHaveURL(/\/app\/billing/);
  });

  test('should navigate to payments from Quick Actions', async ({ page }) => {
    await page.click('text=Record Payment');
    await expect(page).toHaveURL(/\/app\/payments/);
  });

  test('should navigate to imports from Quick Actions', async ({ page }) => {
    await page.click('text=Import Excel');
    await expect(page).toHaveURL(/\/app\/imports/);
  });

  test('should navigate to reports from Quick Actions', async ({ page }) => {
    await page.click('text=Reports');
    await expect(page).toHaveURL(/\/app\/reports/);
  });

  test('should display Recent Activity feed', async ({ page }) => {
    await expect(page.locator('text=Recent Activity')).toBeVisible();
  });
});
