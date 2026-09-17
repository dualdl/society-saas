import { test, expect } from '@playwright/test';

test.describe('Error Handling', () => {

  test('should display 404 page for unknown routes', async ({ page }) => {
    await page.goto('/nonexistent-page');
    await expect(page).toHaveURL(/\/$/);
  });

  test('should handle network errors gracefully', async ({ page }) => {
    await page.goto('/');
    await page.route('**/api/**', route => route.abort('failed'));
    await page.reload();
  });

  test('should handle slow API responses', async ({ page }) => {
    await page.goto('/');
    await page.route('**/api/**', route => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({}),
        delay: 5000,
      });
    });
  });

  test('should handle 500 server errors', async ({ page }) => {
    await page.goto('/');
    await page.route('**/api/**', route => {
      route.fulfill({
        status: 500,
        contentType: 'application/json',
        body: JSON.stringify({ error: 'Internal Server Error' }),
      });
    });
  });

  test('should handle 401 unauthorized responses', async ({ page }) => {
    await page.goto('/');
    await page.route('**/api/**', route => {
      route.fulfill({
        status: 401,
        contentType: 'application/json',
        body: JSON.stringify({ error: 'Unauthorized' }),
      });
    });
  });
});
