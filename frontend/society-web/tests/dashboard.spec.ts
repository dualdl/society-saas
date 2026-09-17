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

  test('should display Quick Actions section', async ({ page }) => {
    await expect(page.locator('text=Quick Actions')).toBeVisible();
    await expect(page.locator('text=Generate Bills')).toBeVisible();
    await expect(page.locator('text=Record Payment')).toBeVisible();
    await expect(page.locator('text=Import Excel')).toBeVisible();
    await expect(page.locator('text=Reports')).toBeVisible();
  });

  test('should navigate to billing from Quick Actions', async ({ page }) => {
    await page.getByRole('button', { name: 'Generate Bills' }).click();
    await expect(page).toHaveURL(/\/app\/billing/);
  });

  test('should navigate to payments from Quick Actions', async ({ page }) => {
    await page.getByRole('button', { name: 'Record Payment' }).click();
    await expect(page).toHaveURL(/\/app\/payments/);
  });

  test('should navigate to imports from Quick Actions', async ({ page }) => {
    await page.getByRole('button', { name: 'Import Excel' }).click();
    await expect(page).toHaveURL(/\/app\/imports/);
  });

  test('should display Recent Activity section', async ({ page }) => {
    await expect(page.locator('text=Recent Activity')).toBeVisible();
  });
});
