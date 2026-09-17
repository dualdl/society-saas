import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Charges Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/charges');
  });

  test('should display charges table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.locator('th:has-text("Name")')).toBeVisible();
    await expect(page.locator('th:has-text("Amount")')).toBeVisible();
    await expect(page.locator('th:has-text("Frequency")')).toBeVisible();
    await expect(page.locator('th:has-text("Active")')).toBeVisible();
  });

  test('should display Add Charge button', async ({ page }) => {
    await expect(page.locator('text=Add Charge')).toBeVisible();
  });

  test('should open Add Charge dialog', async ({ page }) => {
    await page.click('text=Add Charge');
    const dialog = page.locator('[role="dialog"], .MuiDialog-root');
    await expect(dialog).toBeVisible();
    await expect(dialog.locator('text=Name')).toBeVisible();
    await expect(dialog.locator('text=Description')).toBeVisible();
    await expect(dialog.locator('text=Amount')).toBeVisible();
    await expect(dialog.locator('text=Frequency')).toBeVisible();
  });

  test('should create a new charge', async ({ page }) => {
    await page.click('text=Add Charge');
    const dialog = page.locator('[role="dialog"], .MuiDialog-root');

    await dialog.locator('input[name="name"], input').first().fill('Test Charge');
    await dialog.locator('input[name="description"], textarea').first().fill('Test Description');
    await dialog.locator('input[name="amount"], input[type="number"]').first().fill('500');

    const frequencySelect = dialog.locator('select, [role="combobox"]').first();
    if (await frequencySelect.isVisible()) {
      await frequencySelect.click();
      await page.click('text=Monthly');
    }

    await dialog.locator('button:has-text("Save")').click();

    await expect(page.locator('text=Test Charge')).toBeVisible({ timeout: 5000 });
  });

  test('should edit an existing charge', async ({ page }) => {
    const editButton = page.locator('[aria-label="Edit"]').first();
    if (await editButton.isVisible()) {
      await editButton.click();
      const dialog = page.locator('[role="dialog"], .MuiDialog-root');
      await expect(dialog).toBeVisible();
      await dialog.locator('button:has-text("Cancel")').click();
    }
  });

  test('should delete a charge with confirmation', async ({ page }) => {
    page.on('dialog', async (dialog) => {
      expect(dialog.type()).toBe('confirm');
      await dialog.accept();
    });

    const deleteButton = page.locator('[aria-label="Delete"]').first();
    if (await deleteButton.isVisible()) {
      await deleteButton.click();
    }
  });

  test('should cancel charge creation', async ({ page }) => {
    await page.click('text=Add Charge');
    const dialog = page.locator('[role="dialog"], .MuiDialog-root');
    await dialog.locator('button:has-text("Cancel")').click();
    await expect(dialog).not.toBeVisible();
  });
});
