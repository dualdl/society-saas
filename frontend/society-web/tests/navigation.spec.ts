import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin, loginAsSuperAdmin } from './helpers/auth';

test.describe('Navigation - Society User', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
  });

  test('should navigate to all sidebar pages', async ({ page }) => {
    const routes = [
      { menu: 'Dashboard', url: '/app' },
      { menu: 'Flats', url: '/app/flats' },
      { menu: 'Members', url: '/app/members' },
      { menu: 'Billing', url: '/app/billing' },
      { menu: 'Payments', url: '/app/payments' },
      { menu: 'Receipts', url: '/app/receipts' },
      { menu: 'Reports', url: '/app/reports' },
      { menu: 'Import', url: '/app/imports' },
      { menu: 'Audit Trail', url: '/app/audit' },
      { menu: 'Settings', url: '/app/settings' },
    ];

    for (const route of routes) {
      await page.click(`text=${route.menu}`);
      await expect(page).toHaveURL(new RegExp(route.url));
    }
  });

  test('should redirect unauthenticated user to login', async ({ page }) => {
    await page.goto('/app');
    await expect(page).toHaveURL(/\/society\/login/);
  });

  test('should logout and redirect to home', async ({ page }) => {
    await page.click('text=Logout');
    await expect(page).toHaveURL('/');
  });
});

test.describe('Navigation - Super Admin', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSuperAdmin(page);
  });

  test('should navigate to all admin sidebar pages', async ({ page }) => {
    const routes = [
      { menu: 'Dashboard', url: '/admin/dashboard' },
      { menu: 'Societies', url: '/admin/societies' },
      { menu: 'Users', url: '/admin/users' },
      { menu: 'Audit Trail', url: '/admin/audit' },
    ];

    for (const route of routes) {
      await page.click(`text=${route.menu}`);
      await expect(page).toHaveURL(new RegExp(route.url));
    }
  });

  test('should redirect unauthenticated admin to login', async ({ page }) => {
    await page.goto('/admin/dashboard');
    await expect(page).toHaveURL(/\/admin\/login/);
  });
});
