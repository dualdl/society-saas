import { test, expect } from '@playwright/test';

test.describe('Error Handling', () => {

  test('should redirect unknown routes to home', async ({ page }) => {
    await page.goto('/nonexistent-page');
    await expect(page).toHaveURL(/\/$/);
  });

  test('should handle 404 for invalid society slug', async ({ page }) => {
    await page.goto('/s/nonexistent-society-xyz');
    await expect(page.locator('text=Society not found')).toBeVisible({ timeout: 10000 });
  });
});
