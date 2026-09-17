import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin, loginAsSuperAdmin } from './helpers/auth';

test.describe('Navigation - Society User', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
  });

  test('should navigate to Flats page', async ({ page }) => {
    await page.getByRole('button', { name: 'Flats' }).click();
    await expect(page).toHaveURL(/\/app\/flats/);
  });

  test('should navigate to Members page', async ({ page }) => {
    await page.getByRole('button', { name: 'Members' }).click();
    await expect(page).toHaveURL(/\/app\/members/);
  });

  test('should navigate to Billing page', async ({ page }) => {
    await page.getByRole('button', { name: 'Billing' }).click();
    await expect(page).toHaveURL(/\/app\/billing/);
  });

  test('should navigate to Settings page', async ({ page }) => {
    await page.getByRole('button', { name: 'Settings' }).click();
    await expect(page).toHaveURL(/\/app\/settings/);
  });

  test('should redirect unauthenticated user to login', async ({ page }) => {
    await page.goto('/app');
    await expect(page).toHaveURL(/\/society\/login/);
  });
});

test.describe('Navigation - Super Admin', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSuperAdmin(page);
  });

  test('should navigate to Societies page', async ({ page }) => {
    await page.getByRole('button', { name: 'Societies' }).click();
    await expect(page).toHaveURL(/\/admin\/societies/);
  });

  test('should navigate to Users page', async ({ page }) => {
    await page.getByRole('button', { name: 'Users' }).click();
    await expect(page).toHaveURL(/\/admin\/users/);
  });

  test('should redirect unauthenticated admin to login', async ({ page }) => {
    await page.goto('/admin/dashboard');
    await expect(page).toHaveURL(/\/admin\/login/);
  });
});
