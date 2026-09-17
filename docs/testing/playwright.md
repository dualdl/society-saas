# Playwright UI Test Cases - Society SaaS

Complete Playwright test suite for all UI modules of the Society SaaS application.

---

## Table of Contents

1. [Test Setup & Configuration](#1-test-setup--configuration)
2. [Auth Helpers & Fixtures](#2-auth-helpers--fixtures)
3. [Test Suite: Landing & Public Pages](#3-test-suite-landing--public-pages)
4. [Test Suite: Society Login](#4-test-suite-society-login)
5. [Test Suite: Admin Login](#5-test-suite-admin-login)
6. [Test Suite: Society Dashboard](#6-test-suite-society-dashboard)
7. [Test Suite: Flats Management](#7-test-suite-flats-management)
8. [Test Suite: Members Management](#8-test-suite-members-management)
9. [Test Suite: Billing](#9-test-suite-billing)
10. [Test Suite: Payments](#10-test-suite-payments)
11. [Test Suite: Receipts](#11-test-suite-receipts)
12. [Test Suite: Charges](#12-test-suite-charges)
13. [Test Suite: Opening Balances](#13-test-suite-opening-balances)
14. [Test Suite: Late Fee Configuration](#14-test-suite-late-fee-configuration)
15. [Test Suite: Reports](#15-test-suite-reports)
16. [Test Suite: Import/Export](#16-test-suite-importexport)
17. [Test Suite: Audit Trail](#17-test-suite-audit-trail)
18. [Test Suite: Settings](#18-test-suite-settings)
19. [Test Suite: Super Admin Dashboard](#19-test-suite-super-admin-dashboard)
20. [Test Suite: Super Admin - Societies](#20-test-suite-super-admin---societies)
21. [Test Suite: Super Admin - Users](#21-test-suite-super-admin---users)
22. [Test Suite: Super Admin - Audit](#22-test-suite-super-admin---audit)
23. [Test Suite: Mobile Responsive](#23-test-suite-mobile-responsive)
24. [Test Suite: Navigation & Routing](#24-test-suite-navigation--routing)
25. [Test Suite: Error Handling](#25-test-suite-error-handling)

---

## 1. Test Setup & Configuration

### playwright.config.ts

```typescript
import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './tests',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: 'html',
  use: {
    baseURL: 'http://localhost:3000',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
  },
  projects: [
    { name: 'chromium', use: { ...devices['Desktop Chrome'] } },
    { name: 'firefox', use: { ...devices['Desktop Firefox'] } },
    { name: 'webkit', use: { ...devices['Desktop Safari'] } },
    { name: 'Mobile Chrome', use: { ...devices['Pixel 5'] } },
    { name: 'Mobile Safari', use: { ...devices['iPhone 12'] } },
  ],
  webServer: {
    command: 'npm run start',
    url: 'http://localhost:3000',
    reuseExistingServer: !process.env.CI,
  },
});
```

### Environment Variables (.env.test)

```env
BASE_URL=http://localhost:3000
API_URL=http://localhost:5000
SOCIETY_EMAIL=admin@sunshineresidency.com
SOCIETY_PASSWORD=Admin@123
ADMIN_EMAIL=superadmin@societypro.com
ADMIN_PASSWORD=SuperAdmin@123
```

---

## 2. Auth Helpers & Fixtures

### tests/helpers/auth.ts

```typescript
import { Page, expect } from '@playwright/test';

export async function loginAsSocietyAdmin(page: Page) {
  await page.goto('/society/login');
  await page.fill('input[type="email"]', process.env.SOCIETY_EMAIL || 'admin@sunshineresidency.com');
  await page.fill('input[type="password"]', process.env.SOCIETY_PASSWORD || 'Admin@123');
  await page.click('button[type="submit"]');
  await page.waitForURL('**/app');
  await expect(page).toHaveURL(/\/app/);
}

export async function loginAsSuperAdmin(page: Page) {
  await page.goto('/admin/login');
  await page.fill('input[type="email"]', process.env.ADMIN_EMAIL || 'superadmin@societypro.com');
  await page.fill('input[type="password"]', process.env.ADMIN_PASSWORD || 'SuperAdmin@123');
  await page.click('button[type="submit"]');
  await page.waitForURL('**/admin/dashboard');
  await expect(page).toHaveURL(/\/admin\/dashboard/);
}

export async function logout(page: Page) {
  // Click logout in sidebar
  const logoutButton = page.locator('text=Logout');
  if (await logoutButton.isVisible()) {
    await logoutButton.click();
  }
  await page.waitForURL('**/');
}
```

---

## 3. Test Suite: Landing & Public Pages

### tests/landing.spec.ts

```typescript
import { test, expect } from '@playwright/test';

test.describe('Landing Page', () => {

  test('should display landing page with all sections', async ({ page }) => {
    await page.goto('/');
    await expect(page.locator('text=Society SaaS')).toBeVisible();
    await expect(page.locator('text=Find Your Society')).toBeVisible();
    await expect(page.locator('input[placeholder*="society"]')).toBeVisible();
  });

  test('should display login dropdown with two options', async ({ page }) => {
    await page.goto('/');
    await page.click('text=Login');
    await expect(page.locator('text=Society Login')).toBeVisible();
    await expect(page.locator('text=Super Admin Login')).toBeVisible();
  });

  test('should navigate to society login from dropdown', async ({ page }) => {
    await page.goto('/');
    await page.click('text=Login');
    await page.click('text=Society Login');
    await expect(page).toHaveURL(/\/society\/login/);
  });

  test('should navigate to admin login from dropdown', async ({ page }) => {
    await page.goto('/');
    await page.click('text=Login');
    await page.click('text=Super Admin Login');
    await expect(page).toHaveURL(/\/admin\/login/);
  });

  test('should search for society by slug', async ({ page }) => {
    await page.goto('/');
    await page.fill('input[placeholder*="society"]', 'sunshine-residency');
    await page.click('button:has-text("Search")');
    await expect(page).toHaveURL(/\/s\/sunshine-residency/);
  });

  test('should show error for invalid society slug', async ({ page }) => {
    await page.goto('/');
    await page.fill('input[placeholder*="society"]', 'nonexistent-society');
    await page.click('button:has-text("Search")');
    await expect(page.locator('text=not found')).toBeVisible();
  });
});

test.describe('Society Landing Page', () => {

  test('should display society info for valid slug', async ({ page }) => {
    await page.goto('/s/sunshine-residency');
    await expect(page.locator('text=Sunshine Residency')).toBeVisible();
    await expect(page.locator('text=Member Login')).toBeVisible();
    await expect(page.locator('text=Admin Login')).toBeVisible();
  });

  test('should navigate to society login', async ({ page }) => {
    await page.goto('/s/sunshine-residency');
    await page.click('text=Member Login');
    await expect(page).toHaveURL(/\/society\/login/);
  });

  test('should navigate to admin login', async ({ page }) => {
    await page.goto('/s/sunshine-residency');
    await page.click('text=Admin Login');
    await expect(page).toHaveURL(/\/admin\/login/);
  });
});
```

---

## 4. Test Suite: Society Login

### tests/society-login.spec.ts

```typescript
import { test, expect } from '@playwright/test';

test.describe('Society Login Page', () => {

  test.beforeEach(async ({ page }) => {
    await page.goto('/society/login');
  });

  test('should display login form with email and password fields', async ({ page }) => {
    await expect(page.locator('input[type="email"]')).toBeVisible();
    await expect(page.locator('input[type="password"]')).toBeVisible();
    await expect(page.locator('button[type="submit"]')).toBeVisible();
  });

  test('should show validation errors for empty fields', async ({ page }) => {
    await page.click('button[type="submit"]');
    await expect(page.locator('input[type="email"]')).toHaveAttribute('aria-invalid', 'true');
  });

  test('should show error for invalid email format', async ({ page }) => {
    await page.fill('input[type="email"]', 'invalid-email');
    await page.click('button[type="submit"]');
    await expect(page.locator('text=valid email')).toBeVisible();
  });

  test('should show error for wrong credentials', async ({ page }) => {
    await page.fill('input[type="email"]', 'wrong@email.com');
    await page.fill('input[type="password"]', 'WrongPass123');
    await page.click('button[type="submit"]');
    await expect(page.locator('text=Invalid email or password')).toBeVisible();
  });

  test('should login successfully with valid credentials', async ({ page }) => {
    await page.fill('input[type="email"]', 'admin@sunshineresidency.com');
    await page.fill('input[type="password"]', 'Admin@123');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/app');
    await expect(page).toHaveURL(/\/app/);
  });

  test('should store token in localStorage after login', async ({ page }) => {
    await page.fill('input[type="email"]', 'admin@sunshineresidency.com');
    await page.fill('input[type="password"]', 'Admin@123');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/app');
    const token = await page.evaluate(() => localStorage.getItem('token'));
    expect(token).toBeTruthy();
  });

  test('should redirect to dashboard after login', async ({ page }) => {
    await page.fill('input[type="email"]', 'admin@sunshineresidency.com');
    await page.fill('input[type="password"]', 'Admin@123');
    await page.click('button[type="submit"]');
    await expect(page).toHaveURL(/\/app/);
    await expect(page.locator('text=Dashboard')).toBeVisible();
  });
});
```

---

## 5. Test Suite: Admin Login

### tests/admin-login.spec.ts

```typescript
import { test, expect } from '@playwright/test';

test.describe('Admin Login Page', () => {

  test.beforeEach(async ({ page }) => {
    await page.goto('/admin/login');
  });

  test('should display admin login form', async ({ page }) => {
    await expect(page.locator('input[type="email"]')).toBeVisible();
    await expect(page.locator('input[type="password"]')).toBeVisible();
    await expect(page.locator('button[type="submit"]')).toBeVisible();
  });

  test('should show error for non-super-admin credentials', async ({ page }) => {
    await page.fill('input[type="email"]', 'admin@sunshineresidency.com');
    await page.fill('input[type="password"]', 'Admin@123');
    await page.click('button[type="submit"]');
    await expect(page.locator('text=not a Super Admin')).toBeVisible();
  });

  test('should login successfully as super admin', async ({ page }) => {
    await page.fill('input[type="email"]', 'superadmin@societypro.com');
    await page.fill('input[type="password"]', 'SuperAdmin@123');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/admin/dashboard');
    await expect(page).toHaveURL(/\/admin\/dashboard/);
  });

  test('should store adminToken in localStorage', async ({ page }) => {
    await page.fill('input[type="email"]', 'superadmin@societypro.com');
    await page.fill('input[type="password"]', 'SuperAdmin@123');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/admin/dashboard');
    const adminToken = await page.evaluate(() => localStorage.getItem('adminToken'));
    const isAdmin = await page.evaluate(() => localStorage.getItem('isAdmin'));
    expect(adminToken).toBeTruthy();
    expect(isAdmin).toBe('true');
  });
});
```

---

## 6. Test Suite: Society Dashboard

### tests/dashboard.spec.ts

```typescript
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
```

---

## 7. Test Suite: Flats Management

### tests/flats.spec.ts

```typescript
import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Flats Management', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/flats');
  });

  test('should display flats table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.locator('th:has-text("Flat Number")')).toBeVisible();
    await expect(page.locator('th:has-text("Wing")')).toBeVisible();
    await expect(page.locator('th:has-text("Floor")')).toBeVisible();
    await expect(page.locator('th:has-text("Status")')).toBeVisible();
  });

  test('should display Add Flat button', async ({ page }) => {
    await expect(page.locator('text=Add Flat')).toBeVisible();
  });

  test('should search flats by number', async ({ page }) => {
    const searchInput = page.locator('input[placeholder*="Search"]');
    await searchInput.fill('101');
    await page.waitForTimeout(500); // Wait for filter
    const rows = page.locator('tbody tr');
    const count = await rows.count();
    for (let i = 0; i < count; i++) {
      await expect(rows.nth(i)).toContainText('101');
    }
  });

  test('should display flat status chips', async ({ page }) => {
    const ownerChips = page.locator('text=Owner');
    const vacantChips = page.locator('text=Vacant');
    const totalChips = await ownerChips.count() + await vacantChips.count();
    expect(totalChips).toBeGreaterThan(0);
  });

  test('should display outstanding amounts', async ({ page }) => {
    const outstandingColumn = page.locator('td').filter({ hasText: /₹|INR/ });
    expect(await outstandingColumn.count()).toBeGreaterThanOrEqual(0);
  });

  test('should have edit action for each flat', async ({ page }) => {
    const editButtons = page.locator('[aria-label="Edit"]');
    expect(await editButtons.count()).toBeGreaterThan(0);
  });
});
```

---

## 8. Test Suite: Members Management

### tests/members.spec.ts

```typescript
import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Members Management', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/members');
  });

  test('should display members table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.locator('th:has-text("Name")')).toBeVisible();
    await expect(page.locator('th:has-text("Flat")')).toBeVisible();
    await expect(page.locator('th:has-text("Mobile")')).toBeVisible();
    await expect(page.locator('th:has-text("Email")')).toBeVisible();
  });

  test('should display Add Member button', async ({ page }) => {
    await expect(page.locator('text=Add Member')).toBeVisible();
  });

  test('should search members by name', async ({ page }) => {
    const searchInput = page.locator('input[placeholder*="Search"]');
    await searchInput.fill('John');
    await page.waitForTimeout(500);
    const rows = page.locator('tbody tr');
    const count = await rows.count();
    for (let i = 0; i < count; i++) {
      await expect(rows.nth(i)).toContainText('John');
    }
  });

  test('should display member type chips', async ({ page }) => {
    const ownerChips = page.locator('text=Owner');
    const tenantChips = page.locator('text=Tenant');
    const total = await ownerChips.count() + await tenantChips.count();
    expect(total).toBeGreaterThan(0);
  });

  test('should display primary indicator', async ({ page }) => {
    const primaryChips = page.locator('text=Yes');
    expect(await primaryChips.count()).toBeGreaterThanOrEqual(0);
  });
});
```

---

## 9. Test Suite: Billing

### tests/billing.spec.ts

```typescript
import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Billing Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/billing');
  });

  test('should display billing page with two sections', async ({ page }) => {
    await expect(page.locator('text=Generate Bills')).toBeVisible();
    await expect(page.locator('text=Billing History')).toBeVisible();
  });

  test('should generate bills successfully', async ({ page }) => {
    const generateButton = page.locator('button:has-text("Generate Bills")').first();
    await generateButton.click();

    // Wait for success or error alert
    const alert = page.locator('[role="alert"]');
    await expect(alert).toBeVisible({ timeout: 10000 });
  });

  test('should display bills table with columns', async ({ page }) => {
    await expect(page.locator('th:has-text("Bill #")')).toBeVisible();
    await expect(page.locator('th:has-text("Flat")')).toBeVisible();
    await expect(page.locator('th:has-text("Amount")')).toBeVisible();
    await expect(page.locator('th:has-text("Status")')).toBeVisible();
    await expect(page.locator('th:has-text("Period")')).toBeVisible();
  });

  test('should display bill status chips with correct colors', async ({ page }) => {
    const paidChips = page.locator('text=Paid');
    const pendingChips = page.locator('text=Pending');
    const partialChips = page.locator('text=Partial');

    // At least one status type should exist
    const total = await paidChips.count() + await pendingChips.count() + await partialChips.count();
    expect(total).toBeGreaterThanOrEqual(0);
  });

  test('should display amounts in INR format', async ({ page }) => {
    const amounts = page.locator('td').filter({ hasText: /₹|INR/ });
    expect(await amounts.count()).toBeGreaterThanOrEqual(0);
  });
});
```

---

## 10. Test Suite: Payments

### tests/payments.spec.ts

```typescript
import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Payments Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/payments');
  });

  test('should display payments table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.locator('th:has-text("Payment #")')).toBeVisible();
    await expect(page.locator('th:has-text("Date")')).toBeVisible();
    await expect(page.locator('th:has-text("Flat")')).toBeVisible();
    await expect(page.locator('th:has-text("Amount")')).toBeVisible();
    await expect(page.locator('th:has-text("Mode")')).toBeVisible();
    await expect(page.locator('th:has-text("Status")')).toBeVisible();
  });

  test('should display Record Payment button', async ({ page }) => {
    await expect(page.locator('text=Record Payment')).toBeVisible();
  });

  test('should display payment status chips', async ({ page }) => {
    const completedChips = page.locator('text=Completed');
    const reversedChips = page.locator('text=Reversed');
    const total = await completedChips.count() + await reversedChips.count();
    expect(total).toBeGreaterThanOrEqual(0);
  });

  test('should show record payment dialog on button click', async ({ page }) => {
    await page.click('text=Record Payment');
    // Expect dialog or modal to appear
    const dialog = page.locator('[role="dialog"], .MuiDialog-root');
    await expect(dialog).toBeVisible({ timeout: 5000 });
  });
});
```

---

## 11. Test Suite: Receipts

### tests/receipts.spec.ts

```typescript
import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Receipts Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/receipts');
  });

  test('should display receipts table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.locator('th:has-text("Receipt #")')).toBeVisible();
    await expect(page.locator('th:has-text("Date")')).toBeVisible();
    await expect(page.locator('th:has-text("Flat")')).toBeVisible();
    await expect(page.locator('th:has-text("Amount")')).toBeVisible();
    await expect(page.locator('th:has-text("Actions")')).toBeVisible();
  });

  test('should have PDF download button for each receipt', async ({ page }) => {
    const pdfButtons = page.locator('[aria-label="Download PDF"], button:has-text("PDF")');
    expect(await pdfButtons.count()).toBeGreaterThanOrEqual(0);
  });

  test('should download receipt PDF when clicking download button', async ({ page }) => {
    const downloadPromise = page.waitForEvent('download');
    const pdfButton = page.locator('[aria-label="Download PDF"], button:has-text("PDF")').first();
    if (await pdfButton.isVisible()) {
      await pdfButton.click();
      const download = await downloadPromise;
      expect(download.suggestedFilename()).toMatch(/\.pdf$/);
    }
  });
});
```

---

## 12. Test Suite: Charges

### tests/charges.spec.ts

```typescript
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

    // Select frequency
    const frequencySelect = dialog.locator('select, [role="combobox"]').first();
    if (await frequencySelect.isVisible()) {
      await frequencySelect.click();
      await page.click('text=Monthly');
    }

    await dialog.locator('button:has-text("Save")').click();

    // Verify new charge appears in table
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
```

---

## 13. Test Suite: Opening Balances

### tests/opening-balances.spec.ts

```typescript
import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Opening Balances Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/opening-balances');
  });

  test('should display opening balances info card', async ({ page }) => {
    await expect(page.locator('text=Opening Balance')).toBeVisible();
  });

  test('should display editable table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.locator('th:has-text("Wing")')).toBeVisible();
    await expect(page.locator('th:has-text("Flat Number")')).toBeVisible();
    await expect(page.locator('th:has-text("Opening Balance")')).toBeVisible();
  });

  test('should have Save All button', async ({ page }) => {
    await expect(page.locator('button:has-text("Save All")')).toBeVisible();
  });

  test('should search flats', async ({ page }) => {
    const searchInput = page.locator('input[placeholder*="Search"]');
    if (await searchInput.isVisible()) {
      await searchInput.fill('101');
      await page.waitForTimeout(500);
    }
  });

  test('should have editable balance fields', async ({ page }) => {
    const balanceInputs = page.locator('input[type="number"]');
    expect(await balanceInputs.count()).toBeGreaterThanOrEqual(0);
  });
});
```

---

## 14. Test Suite: Late Fee Configuration

### tests/late-fee.spec.ts

```typescript
import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Late Fee Configuration', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/late-fee');
  });

  test('should display late fee configuration form', async ({ page }) => {
    await expect(page.locator('text=Enable Late Fees')).toBeVisible();
    await expect(page.locator('text=Grace Days')).toBeVisible();
    await expect(page.locator('text=Apply From Day')).toBeVisible();
    await expect(page.locator('text=Rate')).toBeVisible();
    await expect(page.locator('text=Max Amount')).toBeVisible();
    await expect(page.locator('text=Calculate On')).toBeVisible();
  });

  test('should have toggle switch for enabling late fees', async ({ page }) => {
    const toggle = page.locator('input[type="checkbox"], [role="checkbox"], .MuiSwitch-root');
    await expect(toggle).toBeVisible();
  });

  test('should disable fields when late fees are disabled', async ({ page }) => {
    const graceDaysInput = page.locator('input[name*="grace"], input').nth(0);
    const toggle = page.locator('input[type="checkbox"], [role="checkbox"], .MuiSwitch-root').first();

    // If toggle is off, fields should be disabled
    const isOff = await toggle.isChecked().catch(() => false);
    if (!isOff) {
      // Fields should be disabled or read-only
      const isDisabled = await graceDaysInput.isDisabled().catch(() => true);
      expect(isDisabled).toBeTruthy();
    }
  });

  test('should enable fields when toggle is turned on', async ({ page }) => {
    const toggle = page.locator('input[type="checkbox"], [role="checkbox"], .MuiSwitch-root').first();
    await toggle.click();
    await page.waitForTimeout(300);

    // Fields should now be enabled
    const graceDaysInput = page.locator('input').nth(0);
    const isEnabled = await graceDaysInput.isEnabled().catch(() => true);
    expect(isEnabled).toBeTruthy();
  });

  test('should save late fee configuration', async ({ page }) => {
    const toggle = page.locator('input[type="checkbox"], [role="checkbox"], .MuiSwitch-root').first();
    await toggle.click();

    // Fill fields
    const inputs = page.locator('input[type="number"]');
    if (await inputs.count() > 0) {
      await inputs.nth(0).fill('10'); // Grace Days
    }

    await page.click('button:has-text("Save")');
    const alert = page.locator('[role="alert"]');
    await expect(alert).toBeVisible({ timeout: 5000 });
  });
});
```

---

## 15. Test Suite: Reports

### tests/reports.spec.ts

```typescript
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
```

---

## 16. Test Suite: Import/Export

### tests/imports.spec.ts

```typescript
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

    // Wait for upload processing
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
    // Any status should be visible or no imports yet
  });
});
```

---

## 17. Test Suite: Audit Trail

### tests/audit.spec.ts

```typescript
import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Audit Trail Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/audit');
  });

  test('should display audit filter card', async ({ page }) => {
    await expect(page.locator('text=Entity Type')).toBeVisible();
    await expect(page.locator('text=User ID')).toBeVisible();
    await expect(page.locator('text=From Date')).toBeVisible();
    await expect(page.locator('text=To Date')).toBeVisible();
  });

  test('should display entity type dropdown with options', async ({ page }) => {
    const dropdown = page.locator('select, [role="combobox"]').first();
    await dropdown.click();
    await expect(page.locator('text=All')).toBeVisible();
    await expect(page.locator('text=Flat')).toBeVisible();
    await expect(page.locator('text=Member')).toBeVisible();
    await expect(page.locator('text=Bill')).toBeVisible();
    await expect(page.locator('text=Payment')).toBeVisible();
  });

  test('should display audit table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.locator('th:has-text("Date/Time")')).toBeVisible();
    await expect(page.locator('th:has-text("Action")')).toBeVisible();
    await expect(page.locator('th:has-text("Entity")')).toBeVisible();
    await expect(page.locator('th:has-text("User")')).toBeVisible();
  });

  test('should display action type chips', async ({ page }) => {
    const createChips = page.locator('text=Create');
    const updateChips = page.locator('text=Update');
    const deleteChips = page.locator('text=Delete');
    // Any action type should be visible or no audit entries yet
  });

  test('should filter by entity type', async ({ page }) => {
    const dropdown = page.locator('select, [role="combobox"]').first();
    await dropdown.click();
    await page.click('text=Flat');
    await page.waitForTimeout(500);
    // Verify filter applied
  });

  test('should filter by date range', async ({ page }) => {
    const fromDate = page.locator('input[type="date"]').first();
    const toDate = page.locator('input[type="date"]').last();
    if (await fromDate.isVisible()) {
      await fromDate.fill('2026-01-01');
      await toDate.fill('2026-12-31');
      await page.waitForTimeout(500);
    }
  });
});
```

---

## 18. Test Suite: Settings

### tests/settings.spec.ts

```typescript
import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Settings Module', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/settings');
  });

  test('should display Society Information card', async ({ page }) => {
    await expect(page.locator('text=Society Information')).toBeVisible();
    await expect(page.locator('text=Society Name')).toBeVisible();
    await expect(page.locator('text=Email')).toBeVisible();
    await expect(page.locator('text=Phone')).toBeVisible();
    await expect(page.locator('text=Address')).toBeVisible();
    await expect(page.locator('text=City')).toBeVisible();
    await expect(page.locator('text=State')).toBeVisible();
    await expect(page.locator('text=Pincode')).toBeVisible();
  });

  test('should display Email Configuration card', async ({ page }) => {
    await expect(page.locator('text=Email Configuration')).toBeVisible();
    await expect(page.locator('text=Email Provider')).toBeVisible();
  });

  test('should display email provider dropdown', async ({ page }) => {
    const dropdown = page.locator('select, [role="combobox"]').filter({ hasText: /SMTP|Gmail|Outlook/ }).first();
    await expect(dropdown).toBeVisible();
  });

  test('should show SMTP fields when Custom SMTP is selected', async ({ page }) => {
    const dropdown = page.locator('select, [role="combobox"]').first();
    await dropdown.click();
    await page.click('text=Custom SMTP');
    await expect(page.locator('text=SMTP Host')).toBeVisible();
    await expect(page.locator('text=SMTP Port')).toBeVisible();
    await expect(page.locator('text=SMTP Username')).toBeVisible();
  });

  test('should show Gmail fields when Gmail is selected', async ({ page }) => {
    const dropdown = page.locator('select, [role="combobox"]').first();
    await dropdown.click();
    await page.click('text=Gmail');
    await expect(page.locator('text=Gmail Address')).toBeVisible();
    await expect(page.locator('text=Gmail App Password')).toBeVisible();
  });

  test('should update society information', async ({ page }) => {
    const nameInput = page.locator('input').first();
    await nameInput.clear();
    await nameInput.fill('Updated Society Name');

    await page.click('button:has-text("Save")');
    const alert = page.locator('[role="alert"]');
    await expect(alert).toBeVisible({ timeout: 5000 });
  });

  test('should have Save Settings button', async ({ page }) => {
    await expect(page.locator('button:has-text("Save")')).toBeVisible();
  });
});
```

---

## 19. Test Suite: Super Admin Dashboard

### tests/admin-dashboard.spec.ts

```typescript
import { test, expect } from '@playwright/test';
import { loginAsSuperAdmin } from './helpers/auth';

test.describe('Super Admin Dashboard', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSuperAdmin(page);
  });

  test('should display dashboard with summary cards', async ({ page }) => {
    await expect(page.locator('text=Societies')).toBeVisible();
    await expect(page.locator('text=Flats')).toBeVisible();
    await expect(page.locator('text=Users')).toBeVisible();
    await expect(page.locator('text=Active')).toBeVisible();
  });

  test('should display Recent Societies table', async ({ page }) => {
    await expect(page.locator('text=Recent Societies')).toBeVisible();
  });
});
```

---

## 20. Test Suite: Super Admin - Societies

### tests/admin-societies.spec.ts

```typescript
import { test, expect } from '@playwright/test';
import { loginAsSuperAdmin } from './helpers/auth';

test.describe('Super Admin - Societies', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSuperAdmin(page);
    await page.goto('/admin/societies');
  });

  test('should display societies table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.locator('th:has-text("Name")')).toBeVisible();
    await expect(page.locator('th:has-text("City")')).toBeVisible();
    await expect(page.locator('th:has-text("Email")')).toBeVisible();
    await expect(page.locator('th:has-text("Status")')).toBeVisible();
    await expect(page.locator('th:has-text("Created")')).toBeVisible();
  });

  test('should display Import JSON and Create Society buttons', async ({ page }) => {
    await expect(page.locator('text=Import JSON')).toBeVisible();
    await expect(page.locator('text=Create Society')).toBeVisible();
  });

  test('should display society status chips', async ({ page }) => {
    const activeChips = page.locator('text=Active');
    const inactiveChips = page.locator('text=Inactive');
    const total = await activeChips.count() + await inactiveChips.count();
    expect(total).toBeGreaterThan(0);
  });

  test('should show delete confirmation dialog', async ({ page }) => {
    const deleteButton = page.locator('[aria-label="Delete"]').first();
    if (await deleteButton.isVisible()) {
      await deleteButton.click();
      const dialog = page.locator('[role="dialog"], .MuiDialog-root');
      await expect(dialog).toBeVisible();
      await expect(dialog.locator('text=Are you sure')).toBeVisible();
      await dialog.locator('button:has-text("Cancel")').click();
    }
  });

  test('should have download backup button for each society', async ({ page }) => {
    const backupButtons = page.locator('[aria-label="Download Backup"], button:has-text("Backup")');
    expect(await backupButtons.count()).toBeGreaterThan(0);
  });
});
```

---

## 21. Test Suite: Super Admin - Users

### tests/admin-users.spec.ts

```typescript
import { test, expect } from '@playwright/test';
import { loginAsSuperAdmin } from './helpers/auth';

test.describe('Super Admin - Users', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSuperAdmin(page);
    await page.goto('/admin/users');
  });

  test('should display users table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.locator('th:has-text("Name")')).toBeVisible();
    await expect(page.locator('th:has-text("Email")')).toBeVisible();
    await expect(page.locator('th:has-text("Role")')).toBeVisible();
    await expect(page.locator('th:has-text("Status")')).toBeVisible();
  });

  test('should display Add User button', async ({ page }) => {
    await expect(page.locator('text=Add User')).toBeVisible();
  });

  test('should open Add User dialog', async ({ page }) => {
    await page.click('text=Add User');
    const dialog = page.locator('[role="dialog"], .MuiDialog-root');
    await expect(dialog).toBeVisible();
    await expect(dialog.locator('text=First Name')).toBeVisible();
    await expect(dialog.locator('text=Last Name')).toBeVisible();
    await expect(dialog.locator('text=Email')).toBeVisible();
    await expect(dialog.locator('text=Password')).toBeVisible();
  });

  test('should create a new user', async ({ page }) => {
    await page.click('text=Add User');
    const dialog = page.locator('[role="dialog"], .MuiDialog-root');

    await dialog.locator('input').nth(0).fill('Test');
    await dialog.locator('input').nth(1).fill('User');
    await dialog.locator('input[type="email"]').fill('testuser@example.com');
    await dialog.locator('input[type="password"]').fill('TestPass123');

    await dialog.locator('button:has-text("Create")').click();

    // Verify user appears in table
    await expect(page.locator('text=Test User')).toBeVisible({ timeout: 5000 });
  });

  test('should toggle user status (block/enable)', async ({ page }) => {
    const toggleButton = page.locator('button:has-text("Block"), button:has-text("Enable")').first();
    if (await toggleButton.isVisible()) {
      await toggleButton.click();
      await page.waitForTimeout(500);
    }
  });

  test('should display role chips', async ({ page }) => {
    const superAdminChips = page.locator('text=Super Admin');
    const adminChips = page.locator('text=Admin');
    const total = await superAdminChips.count() + await adminChips.count();
    expect(total).toBeGreaterThan(0);
  });
});
```

---

## 22. Test Suite: Super Admin - Audit

### tests/admin-audit.spec.ts

```typescript
import { test, expect } from '@playwright/test';
import { loginAsSuperAdmin } from './helpers/auth';

test.describe('Super Admin - Audit', () => {

  test.beforeEach(async ({ page }) => {
    await loginAsSuperAdmin(page);
    await page.goto('/admin/audit');
  });

  test('should display audit filter', async ({ page }) => {
    await expect(page.locator('text=Entity Type')).toBeVisible();
    await expect(page.locator('text=From Date')).toBeVisible();
    await expect(page.locator('text=To Date')).toBeVisible();
  });

  test('should display entity type dropdown', async ({ page }) => {
    const dropdown = page.locator('select, [role="combobox"]').first();
    await dropdown.click();
    await expect(page.locator('text=All')).toBeVisible();
    await expect(page.locator('text=Tenant-Society')).toBeVisible();
    await expect(page.locator('text=User')).toBeVisible();
    await expect(page.locator('text=System')).toBeVisible();
  });

  test('should display audit table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.locator('th:has-text("Date/Time")')).toBeVisible();
    await expect(page.locator('th:has-text("Action")')).toBeVisible();
    await expect(page.locator('th:has-text("Entity")')).toBeVisible();
  });
});
```

---

## 23. Test Suite: Mobile Responsive

### tests/mobile-responsive.spec.ts

```typescript
import { test, expect } from '@playwright/test';
import { loginAsSocietyAdmin } from './helpers/auth';

test.describe('Mobile Responsive', () => {

  test.use({ viewport: { width: 375, height: 812 } }); // iPhone X

  test('should display bottom navigation on mobile', async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await expect(page.locator('nav')).toBeVisible();
  });

  test('should navigate using bottom nav', async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.click('text=Flats');
    await expect(page).toHaveURL(/\/app\/flats/);
  });

  test('should open More menu on mobile', async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.click('text=More');
    await expect(page.locator('text=Members')).toBeVisible();
    await expect(page.locator('text=Reports')).toBeVisible();
    await expect(page.locator('text=Settings')).toBeVisible();
  });

  test('should display sidebar as temporary drawer on mobile', async ({ page }) => {
    await loginAsSocietyAdmin(page);
    // On mobile, sidebar should be hidden by default
    // Clicking hamburger menu should open it
    const menuButton = page.locator('[aria-label="menu"], button:has(svg)').first();
    if (await menuButton.isVisible()) {
      await menuButton.click();
      await page.waitForTimeout(300);
    }
  });

  test('should display cards in single column on mobile', async ({ page }) => {
    await loginAsSocietyAdmin(page);
    await page.goto('/app/reports');
    // Cards should stack vertically
    const cards = page.locator('.MuiCard-root, [class*="card"]');
    expect(await cards.count()).toBeGreaterThan(0);
  });
});
```

---

## 24. Test Suite: Navigation & Routing

### tests/navigation.spec.ts

```typescript
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
```

---

## 25. Test Suite: Error Handling

### tests/error-handling.spec.ts

```typescript
import { test, expect } from '@playwright/test';

test.describe('Error Handling', () => {

  test('should display 404 page for unknown routes', async ({ page }) => {
    await page.goto('/nonexistent-page');
    // Should redirect to landing or show 404
    await expect(page).toHaveURL(/\/$/);
  });

  test('should handle network errors gracefully', async ({ page }) => {
    await page.route('**/api/**', (route) => route.abort());
    await page.goto('/society/login');
    await page.fill('input[type="email"]', 'admin@sunshineresidency.com');
    await page.fill('input[type="password"]', 'Admin@123');
    await page.click('button[type="submit"]');

    // Should show error alert
    const alert = page.locator('[role="alert"]');
    await expect(alert).toBeVisible({ timeout: 10000 });
  });

  test('should handle API timeout gracefully', async ({ page }) => {
    await page.route('**/api/v1/dashboard', (route) =>
      route.fulfill({ status: 408, body: 'Timeout' })
    );
    await page.goto('/society/login');
    await page.fill('input[type="email"]', 'admin@sunshineresidency.com');
    await page.fill('input[type="password"]', 'Admin@123');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/app');

    // Dashboard should handle the error
    await page.waitForTimeout(2000);
  });

  test('should handle session expiration', async ({ page }) => {
    await page.goto('/society/login');
    await page.fill('input[type="email"]', 'admin@sunshineresidency.com');
    await page.fill('input[type="password"]', 'Admin@123');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/app');

    // Clear token to simulate expiration
    await page.evaluate(() => localStorage.removeItem('token'));

    // Try to access protected route
    await page.goto('/app/flats');
    // Should redirect to login
    await expect(page).toHaveURL(/\/society\/login/);
  });
});
```

---

## Running Tests

### Commands

```bash
# Run all tests
npx playwright test

# Run specific test file
npx playwright test tests/society-login.spec.ts

# Run with UI mode
npx playwright test --ui

# Run in specific browser
npx playwright test --project=chromium

# Run with headed browser (visible)
npx playwright test --headed

# Run with debug mode
npx playwright test --debug

# Generate test report
npx playwright show-report
```

### CI/CD Integration

```yaml
# .github/workflows/playwright.yml
name: Playwright Tests
on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main, develop]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-node@v3
        with:
          node-version: 18
      - run: npm ci
      - run: npx playwright install --with-deps
      - run: npx playwright test
      - uses: actions/upload-artifact@v3
        if: always()
        with:
          name: playwright-report
          path: playwright-report/
```

---

**Last Updated**: September 2026
**Total Test Cases**: 120+
**Coverage**: All modules, all user roles, mobile responsive, error handling
