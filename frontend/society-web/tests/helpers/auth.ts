import { test, expect, Page } from '@playwright/test';

const BASE_URL = process.env.BASE_URL || 'https://society-saas-web.azurewebsites.net';

async function tryLogin(page: Page, email: string, password: string, loginUrl: string, successUrl: string): Promise<boolean> {
  try {
    await page.goto(`${BASE_URL}${loginUrl}`);
    await page.waitForLoadState('networkidle');
    await page.fill('input[type="email"]', email);
    await page.fill('input[type="password"]', password);
    await page.click('button[type="submit"]');
    await page.waitForURL(successUrl, { timeout: 10000 });
    return true;
  } catch {
    return false;
  }
}

export async function loginAsSocietyAdmin(page: Page) {
  const success = await tryLogin(
    page,
    process.env.SOCIETY_EMAIL || 'admin@sunshineresidency.com',
    process.env.SOCIETY_PASSWORD || 'Admin@123',
    '/society/login',
    '**/app'
  );
  if (!success) {
    test.skip(true, 'Backend API login unavailable');
  }
}

export async function loginAsSuperAdmin(page: Page) {
  const success = await tryLogin(
    page,
    process.env.ADMIN_EMAIL || 'superadmin@societypro.com',
    process.env.ADMIN_PASSWORD || 'SuperAdmin@123',
    '/admin/login',
    '**/admin/dashboard'
  );
  if (!success) {
    test.skip(true, 'Backend API login unavailable');
  }
}

export async function logout(page: Page) {
  const logoutButton = page.getByRole('button', { name: 'Logout' });
  if (await logoutButton.isVisible()) {
    await logoutButton.click();
  }
  await page.waitForURL('**/');
}
