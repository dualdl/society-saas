# Society SaaS - Complete Application Reference

This document provides a comprehensive reference of every module, page, form, dialog, API endpoint, and user role in the Society SaaS application. Used as the basis for Playwright UI test generation.

---

## Table of Contents

1. [Login Credentials](#1-login-credentials)
2. [User Roles & Access](#2-user-roles--access)
3. [Navigation Structure](#3-navigation-structure)
4. [Module: Landing & Public Pages](#4-module-landing--public-pages)
5. [Module: Authentication](#5-module-authentication)
6. [Module: Dashboard (Society)](#6-module-dashboard-society)
7. [Module: Flats Management](#7-module-flats-management)
8. [Module: Members Management](#8-module-members-management)
9. [Module: Billing](#9-module-billing)
10. [Module: Payments](#10-module-payments)
11. [Module: Receipts](#11-module-receipts)
12. [Module: Charges](#12-module-charges)
13. [Module: Opening Balances](#13-module-opening-balances)
14. [Module: Late Fee Configuration](#14-module-late-fee-configuration)
15. [Module: Reports](#15-module-reports)
16. [Module: Import/Export](#16-module-importexport)
17. [Module: Audit Trail](#17-module-audit-trail)
18. [Module: Settings](#18-module-settings)
19. [Module: Super Admin Dashboard](#19-module-super-admin-dashboard)
20. [Module: Super Admin - Societies](#20-module-super-admin---societies)
21. [Module: Super Admin - Users](#21-module-super-admin---users)
22. [Module: Super Admin - Audit](#22-module-super-admin---audit)
23. [Module: Resident Dashboard](#23-module-resident-dashboard)
24. [API Endpoints Reference](#24-api-endpoints-reference)

---

## 1. Login Credentials

### Society User Login

| Field | Value |
|-------|-------|
| **URL** | `http://localhost:3000/society/login` |
| **Email** | `admin@sunshineresidency.com` |
| **Password** | `Admin@123` |
| **Redirect** | `/app` (Dashboard) |
| **Token Storage** | `localStorage.token`, `localStorage.user`, `localStorage.tenantId`, `localStorage.tenantName` |

### Super Admin Login

| Field | Value |
|-------|-------|
| **URL** | `http://localhost:3000/admin/login` |
| **Email** | `superadmin@societypro.com` |
| **Password** | `SuperAdmin@123` |
| **Redirect** | `/admin/dashboard` |
| **Token Storage** | `localStorage.adminToken`, `localStorage.isAdmin = 'true'` |

### Demo Society Access

| Field | Value |
|-------|-------|
| **Society URL** | `http://localhost:3000/s/sunshine-residency` |
| **Society Name** | Sunshine Residency |
| **Slug** | `sunshine-residency` |

---

## 2. User Roles & Access

| Role | Route Prefix | Access Level |
|------|-------------|--------------|
| **Super Admin** | `/admin/*` | Platform-wide: all societies, users, audit |
| **Admin** | `/app/*` | Society-scoped: all modules for their society |
| **Resident** | `/resident` | Flat-scoped: own flat, bills, payments |

### Role Assignment
- Super Admin: `User.IsSuperAdmin = true`
- Society Admin: `UserTenant.Role = "Admin"` (auto-created when society is created)
- Resident: `UserTenant.Role = "Resident"` or "Member"

---

## 3. Navigation Structure

### Society Sidebar (Desktop)

| Menu Item | Route | Icon |
|-----------|-------|------|
| Dashboard | `/app` | Dashboard |
| Flats | `/app/flats` | Home |
| Members | `/app/members` | People |
| Billing | `/app/billing` | Receipt |
| Payments | `/app/payments` | Payment |
| Receipts | `/app/receipts` | Description |
| Reports | `/app/reports` | Assessment |
| Import | `/app/imports` | Upload |
| Audit Trail | `/app/audit` | History |
| Settings | `/app/settings` | Settings |
| Logout | - | Logout |

### Mobile Bottom Navigation

| Tab | Route |
|-----|-------|
| Home | `/app` |
| Flats | `/app/flats` |
| Billing | `/app/billing` |
| Payments | `/app/payments` |
| More | Menu: Members, Reports, Import, Audit Trail, Settings |

### Admin Sidebar

| Menu Item | Route | Icon |
|-----------|-------|------|
| Dashboard | `/admin/dashboard` | Dashboard |
| Societies | `/admin/societies` | Apartment |
| Users | `/admin/users` | People |
| Audit Trail | `/admin/audit` | History |
| Settings | `/admin/settings` | Settings |
| Logout | - | Logout |

---

## 4. Module: Landing & Public Pages

### 4.1 Landing Page (`/`)

**Component**: `Landing.tsx`

**Elements**:
- Navigation bar with Society SaaS logo
- Login dropdown button with two options:
  - "Society Login" -> navigates to `/society/login`
  - "Super Admin Login" -> navigates to `/admin/login`
- Hero section with headline and CTA
- Feature cards section
- Pricing section
- "Find Your Society" search section:
  - Text input for society slug
  - Search button -> navigates to `/s/{slug}`
- Footer with links

**Test Data**:
| Input | Expected Result |
|-------|----------------|
| Valid slug (`sunshine-residency`) | Navigate to `/s/sunshine-residency` |
| Invalid slug | Show "Society not found" or error |

### 4.2 Society Landing Page (`/s/:slug`)

**Component**: `SocietyLanding.tsx`

**Elements**:
- Society name and logo
- City, State display
- Feature cards (Billing, Payments, Receipts, Mobile)
- "Member Login" button -> `/society/login`
- "Admin Login" button -> `/admin/login`

---

## 5. Module: Authentication

### 5.1 Society Login (`/society/login`)

**Component**: `SocietyLogin.tsx`

**Form Fields**:
| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Email | text, type="email" | Yes | Valid email format |
| Password | password | Yes | Non-empty |

**Buttons**:
- "Login" (submit)

**Behavior**:
- On success: stores `token`, `user`, `tenantId`, `tenantName` in localStorage, redirects to `/app`
- On error: shows error alert "Invalid email or password"
- Loading state during API call

**API**: `POST /api/v1/auth/login` with `{ email, password }`

### 5.2 Admin Login (`/admin/login`)

**Component**: `AdminLogin.tsx`

**Form Fields**:
| Field | Type | Required |
|-------|------|----------|
| Email | text, type="email" | Yes |
| Password | password | Yes |

**Behavior**:
- On success: checks `isSuperAdmin` flag
- If not super admin: shows error "This account is not a Super Admin account"
- If super admin: stores `adminToken`, `isAdmin = 'true'`, redirects to `/admin/dashboard`

**API**: `POST /api/v1/auth/login` with `{ email, password }`

---

## 6. Module: Dashboard (Society)

**Route**: `/app`
**Component**: `Dashboard.tsx`

### Summary Cards
| Card | Data |
|------|------|
| Total Flats | Count |
| Total Members | Count |
| Collection | Amount + Percentage |
| Outstanding | Amount |

### Quick Actions
| Button | Action |
|--------|--------|
| Generate Bills | Navigates to `/app/billing` |
| Record Payment | Navigates to `/app/payments` |
| Import Excel | Navigates to `/app/imports` |
| Reports | Navigates to `/app/reports` |

### Recent Activity Feed
- List of recent actions with timestamp, action type, entity

**API**: `GET /api/v1/dashboard`

---

## 7. Module: Flats Management

**Route**: `/app/flats`
**Component**: `Flats.tsx`

### Table Columns
| Column | Type | Sortable |
|--------|------|----------|
| Wing | text | Yes |
| Flat Number | text | Yes |
| Floor | number | Yes |
| Type | text | Yes |
| Area (sqft) | number | Yes |
| Status | chip (Owner/Vacant) | Yes |
| Members | number | Yes |
| Outstanding | currency (INR) | Yes |
| Actions | Edit icon | - |

### Search
- Text input: filters flats by number, wing, or member name

### Add Flat Button
- Opens Add Flat Dialog (implied)

### API
- `GET /api/v1/flats` - List flats
- `POST /api/v1/flats` - Create flat
- `PUT /api/v1/flats/{id}` - Update flat
- `DELETE /api/v1/flats/{id}` - Delete flat

---

## 8. Module: Members Management

**Route**: `/app/members`
**Component**: `Members.tsx`

### Table Columns
| Column | Type |
|--------|------|
| Flat | text |
| Name | text |
| Mobile | text |
| Email | text |
| Type | chip (Owner/Tenant) |
| Primary | chip (Yes/No) |
| Actions | Edit icon |

### Search
- Text input: filters members by name, flat, or email

### Add Member Button
- Opens Add Member Dialog (implied)

### API
- `GET /api/v1/members` - List members
- `POST /api/v1/members` - Create member
- `PUT /api/v1/members/{id}` - Update member
- `DELETE /api/v1/members/{id}` - Delete member

---

## 9. Module: Billing

**Route**: `/app/billing`
**Component**: `Billing.tsx`

### Two Action Cards

**Card 1: Generate Bills**
- Button: "Generate Bills"
- Action: Calls `POST /api/v1/bills/bulk-generate`
- Shows success/error alert after generation

**Card 2: Billing History**
- Table of recent bills

### Bills Table Columns
| Column | Type |
|--------|------|
| Bill # | text |
| Flat | text |
| Amount | currency (INR) |
| Status | chip (Paid=green, Partial=yellow, Pending=red) |
| Period | text |

### API
- `GET /api/v1/bills` - List bills
- `POST /api/v1/bills/bulk-generate` - Bulk generate bills
- `POST /api/v1/bills/generate` - Generate single bill
- `PUT /api/v1/bills/{id}/status` - Update status
- `PUT /api/v1/bills/{id}/cancel` - Cancel bill

---

## 10. Module: Payments

**Route**: `/app/payments`
**Component**: `Payments.tsx`

### Table Columns
| Column | Type |
|--------|------|
| Payment # | text |
| Date | date |
| Flat | text |
| Amount | currency (INR) |
| Mode | text |
| Status | chip (Completed=green, Reversed=red) |

### Record Payment Button
- Opens Record Payment Dialog (implied)

### API
- `GET /api/v1/payments` - List payments
- `POST /api/v1/payments` - Record payment
- `POST /api/v1/payments/{id}/reverse` - Reverse payment

---

## 11. Module: Receipts

**Route**: `/app/receipts`
**Component**: `Receipts.tsx`

### Table Columns
| Column | Type |
|--------|------|
| Receipt # | text |
| Date | date |
| Flat | text |
| Amount | currency (INR) |
| Mode | text |
| Actions | PDF download button |

### PDF Download
- Click PDF icon -> Downloads receipt as PDF blob

### API
- `GET /api/v1/receipts` - List receipts
- `GET /api/v1/receipts/{id}/pdf` - Download PDF

---

## 12. Module: Charges

**Route**: `/app/charges`
**Component**: `Charges.tsx`

### Charges Master Table
| Column | Type |
|--------|------|
| Name | text |
| Description | text |
| Amount | currency (INR) |
| Frequency | text |
| Active | chip (Yes/No) |
| Actions | Edit icon, Delete icon |

### Add Charge Dialog
| Field | Type | Required |
|-------|------|----------|
| Name | text | Yes |
| Description | text | No |
| Amount | number | Yes |
| Frequency | dropdown: Monthly, Quarterly, Yearly, OneTime | Yes |

**Buttons**: Cancel, Save

### Delete Confirmation
- Native `window.confirm("Delete this charge?")`

### API
- `GET /api/v1/charges` - List charges
- `POST /api/v1/charges` - Create charge
- `PUT /api/v1/charges/{id}` - Update charge
- `DELETE /api/v1/charges/{id}` - Delete charge

---

## 13. Module: Opening Balances

**Route**: `/app/opening-balances`
**Component**: `OpeningBalances.tsx`

### Info Card
- Explains positive (credit) vs negative (debit) values

### Search Box
- Filters flats by number

### Editable Table
| Column | Type |
|--------|------|
| Wing | text (read-only) |
| Flat Number | text (read-only) |
| Opening Balance | number input (step 0.01) |

### Save All Button
- Saves all opening balances via `POST /api/v1/opening-balances`

### API
- `GET /api/v1/opening-balances` - Get all
- `POST /api/v1/opening-balances` - Save batch

---

## 14. Module: Late Fee Configuration

**Route**: `/app/late-fee`
**Component**: `LateFee.tsx`

### Configuration Form
| Field | Type | Default |
|-------|------|---------|
| Enable Late Fees | Switch toggle | false |
| Grace Days | number | 0 |
| Apply From Day | number | 0 |
| Rate (%) | number | 0 |
| Max Amount (INR) | number | 0 |
| Calculate On | select: Outstanding Amount / Total Bill Amount | Outstanding Amount |

**Behavior**:
- All fields disabled when "Enable Late Fees" is OFF
- Save button submits configuration

### API
- `GET /api/v1/late-payment-rules` - Get config
- `POST /api/v1/late-payment-rules` - Save config

---

## 15. Module: Reports

**Route**: `/app/reports`
**Component**: `Reports.tsx`

### Report Cards Grid (6 cards)
| Report | Action | API |
|--------|--------|-----|
| Flat Register | Download Excel | `GET /api/v1/reports/flats/excel` |
| Member Register | Download Excel | `GET /api/v1/reports/members/excel` |
| Bill Register | Download Excel | `GET /api/v1/reports/bills/excel` |
| Payment Register | Download Excel | `GET /api/v1/reports/payments/excel` |
| Outstanding Report | Download Excel | `GET /api/v1/reports/outstanding/excel` |
| Audit Trail | Navigates to `/app/audit` | - |

---

## 16. Module: Import/Export

**Route**: `/app/imports`
**Component**: `Imports.tsx`

### Upload Card
- File input: accepts `.xlsx`, `.xls`, `.csv`
- Upload button -> `POST /api/v1/imports/upload`

### Download Template Card
- Button: "Download Template" -> `GET /api/v1/imports/template`

### Import History Table
| Column | Type |
|--------|------|
| File | text |
| Type | text |
| Status | chip (completed=green, processing=yellow, failed=red) |
| Total | number |
| Success | number |
| Errors | number |
| Date | date |
| Actions | View Errors icon, Confirm button |

### Error Preview Dialog
- Table with Row number and Error message
- Close button

### Import Flow
1. Upload file -> Validation happens
2. If errors: show error dialog
3. If valid: show Confirm button
4. Click Confirm -> `POST /api/v1/imports/{jobId}/confirm`

### API
- `GET /api/v1/imports/template` - Download template
- `POST /api/v1/imports/upload` - Upload and validate
- `GET /api/v1/imports/{jobId}` - Get job status
- `POST /api/v1/imports/{jobId}/confirm` - Execute import

---

## 17. Module: Audit Trail

**Route**: `/app/audit`
**Component**: `Audit.tsx`

### Filter Card
| Field | Type |
|-------|------|
| Entity Type | dropdown: All, Flat, Member, Bill, Payment, Receipt, Charge |
| User ID | text |
| From Date | date picker |
| To Date | date picker |

### Audit Table
| Column | Type |
|--------|------|
| Date/Time | datetime |
| Action | chip (Create=green, Update=blue, Delete=red) |
| Entity | text |
| User | text |
| Changes | text (JSON diff) |

### API
- `GET /api/v1/audit` - Get audit logs

---

## 18. Module: Settings

**Route**: `/app/settings`
**Component**: `Settings.tsx`

### Society Information Card
| Field | Type |
|-------|------|
| Society Name | text |
| Email | text, type="email" |
| Phone | text |
| Address | text |
| City | text |
| State | text |
| Pincode | text |
| Logo URL | text |

### Email Configuration Card
| Field | Type |
|-------|------|
| Email Provider | dropdown: Custom SMTP, Gmail, Outlook |

**Custom SMTP Fields** (shown when provider = Custom SMTP):
| Field | Type |
|-------|------|
| SMTP Host | text |
| SMTP Port | number |
| SMTP Username | text |
| SMTP Password | password |
| Use SSL/TLS | Switch toggle |

**Gmail Fields** (shown when provider = Gmail):
| Field | Type |
|-------|------|
| Gmail Address | text, type="email" |
| Gmail App Password | password |

**Outlook Fields** (shown when provider = Outlook):
| Field | Type |
|-------|------|
| Outlook Email | text, type="email" |
| Outlook Password | password |

### Save Settings Button
- Saves all settings via `PUT /api/v1/settings`

### API
- `GET /api/v1/settings` - Get settings
- `PUT /api/v1/settings` - Update settings

---

## 19. Module: Super Admin Dashboard

**Route**: `/admin/dashboard`
**Component**: `admin/Dashboard.tsx`

### Summary Cards
| Card | Data |
|------|------|
| Societies | Total count |
| Flats | Total count across all societies |
| Users | Total user count |
| Active | Active societies count |

### Recent Societies Table
| Column | Type |
|--------|------|
| Name | text |
| Status | chip (Active/Inactive) |
| Created | date |

### API
- `GET /api/v1/superadmin/dashboard`

---

## 20. Module: Super Admin - Societies

**Route**: `/admin/societies`
**Component**: `admin/Societies.tsx`

### Table Columns
| Column | Type |
|--------|------|
| Name | text |
| City | text |
| Email | text |
| Status | chip (Active=green, Inactive=red) |
| Created | date |
| Actions | Download Backup JSON icon, Delete icon |

### Buttons
- "Import JSON" -> Opens file upload for JSON import
- "Create Society" -> Opens Create Society dialog

### Delete Confirmation Dialog
- Warning text: "Are you sure you want to delete this society?"
- Buttons: Cancel, Delete

### Create Society (API)
Fields sent: Name, Address, City, State, PinCode, Phone, Email, Slug

### API
- `GET /api/v1/superadmin/societies` - List societies
- `POST /api/v1/superadmin/societies` - Create society
- `DELETE /api/v1/superadmin/societies/{id}` - Delete society
- `POST /api/v1/superadmin/societies/import` - Import from JSON
- `GET /api/v1/superadmin/societies/{id}/backup` - Download backup

---

## 21. Module: Super Admin - Users

**Route**: `/admin/users`
**Component**: `admin/Users.tsx`

### Table Columns
| Column | Type |
|--------|------|
| Name | text |
| Email | text |
| Role | chip (Super Admin=purple, Admin=blue) |
| Status | chip (Active=green, Disabled=red) |
| Created | date |
| Actions | Block/Enable toggle button |

### Add User Dialog
| Field | Type | Required |
|-------|------|----------|
| First Name | text | Yes |
| Last Name | text | Yes |
| Email | text, type="email" | Yes |
| Password | password | Yes |

**Buttons**: Cancel, Create

### API
- `GET /api/v1/superadmin/users` - List users
- `POST /api/v1/superadmin/users` - Create user
- `PUT /api/v1/superadmin/users/{id}` - Toggle user status

---

## 22. Module: Super Admin - Audit

**Route**: `/admin/audit`
**Component**: `admin/Audit.tsx`

### Filter Card
| Field | Type |
|-------|------|
| Entity Type | dropdown: All, Tenant-Society, User, System |
| From Date | date picker |
| To Date | date picker |

### Audit Table
| Column | Type |
|--------|------|
| Date/Time | datetime |
| Action | text |
| Entity | text |
| User | text |
| Changes | text |

### API
- `GET /api/v1/superadmin/audit`

---

## 23. Module: Resident Dashboard

**Route**: `/resident`
**Component**: `ResidentDashboard.tsx`

### Mobile-Optimized View
- My Flat details
- Current Bill amount
- Recent Payment history
- Bottom Navigation: Home, Flats, Billing, Payments, More

---

## 24. API Endpoints Reference

### Authentication
| Method | Endpoint | Auth |
|--------|----------|------|
| POST | `/api/v1/auth/login` | No |
| POST | `/api/v1/auth/register` | No |
| POST | `/api/v1/auth/request-otp` | No |
| POST | `/api/v1/auth/verify-otp` | No |
| POST | `/api/v1/auth/refresh-token` | No |
| GET | `/api/v1/auth/me` | JWT |
| POST | `/api/v1/auth/change-password` | JWT |

### Flats
| Method | Endpoint |
|--------|----------|
| GET | `/api/v1/flats` |
| GET | `/api/v1/flats/{id}` |
| POST | `/api/v1/flats` |
| PUT | `/api/v1/flats/{id}` |
| DELETE | `/api/v1/flats/{id}` |

### Members
| Method | Endpoint |
|--------|----------|
| GET | `/api/v1/members` |
| GET | `/api/v1/members/{id}` |
| POST | `/api/v1/members` |
| PUT | `/api/v1/members/{id}` |
| DELETE | `/api/v1/members/{id}` |

### Bills
| Method | Endpoint |
|--------|----------|
| GET | `/api/v1/bills` |
| GET | `/api/v1/bills/{id}` |
| POST | `/api/v1/bills/generate` |
| POST | `/api/v1/bills/bulk-generate` |
| PUT | `/api/v1/bills/{id}/status` |
| PUT | `/api/v1/bills/{id}/cancel` |

### Payments
| Method | Endpoint |
|--------|----------|
| GET | `/api/v1/payments` |
| GET | `/api/v1/payments/{id}` |
| POST | `/api/v1/payments` |
| POST | `/api/v1/payments/{id}/reverse` |

### Receipts
| Method | Endpoint |
|--------|----------|
| GET | `/api/v1/receipts` |
| GET | `/api/v1/receipts/{id}` |
| GET | `/api/v1/receipts/{id}/pdf` |

### Charges
| Method | Endpoint |
|--------|----------|
| GET | `/api/v1/charges` |
| GET | `/api/v1/charges/{id}` |
| POST | `/api/v1/charges` |
| PUT | `/api/v1/charges/{id}` |
| DELETE | `/api/v1/charges/{id}` |

### Dashboard
| Method | Endpoint |
|--------|----------|
| GET | `/api/v1/dashboard` |

### Reports
| Method | Endpoint |
|--------|----------|
| GET | `/api/v1/reports/revenue` |
| GET | `/api/v1/reports/outstanding` |
| GET | `/api/v1/reports/bills/excel` |
| GET | `/api/v1/reports/payments/excel` |
| GET | `/api/v1/reports/outstanding/excel` |
| GET | `/api/v1/reports/members/excel` |
| GET | `/api/v1/reports/flats/excel` |

### Imports
| Method | Endpoint |
|--------|----------|
| GET | `/api/v1/imports/template` |
| POST | `/api/v1/imports/upload` |
| GET | `/api/v1/imports/{jobId}` |
| POST | `/api/v1/imports/{jobId}/confirm` |

### Audit
| Method | Endpoint |
|--------|----------|
| GET | `/api/v1/audit` |

### Settings
| Method | Endpoint |
|--------|----------|
| GET | `/api/v1/settings` |
| PUT | `/api/v1/settings` |

### Opening Balances
| Method | Endpoint |
|--------|----------|
| GET | `/api/v1/opening-balances` |
| POST | `/api/v1/opening-balances` |

### Late Payment Rules
| Method | Endpoint |
|--------|----------|
| GET | `/api/v1/late-payment-rules` |
| POST | `/api/v1/late-payment-rules` |

### Super Admin
| Method | Endpoint |
|--------|----------|
| GET | `/api/v1/superadmin/dashboard` |
| GET | `/api/v1/superadmin/societies` |
| POST | `/api/v1/superadmin/societies` |
| DELETE | `/api/v1/superadmin/societies/{id}` |
| POST | `/api/v1/superadmin/societies/import` |
| GET | `/api/v1/superadmin/societies/{id}/backup` |
| GET | `/api/v1/superadmin/users` |
| POST | `/api/v1/superadmin/users` |
| PUT | `/api/v1/superadmin/users/{id}` |
| GET | `/api/v1/superadmin/audit` |

### Wings
| Method | Endpoint |
|--------|----------|
| GET | `/api/v1/wings` |
| POST | `/api/v1/wings` |
| PUT | `/api/v1/wings/{id}` |
| DELETE | `/api/v1/wings/{id}` |

### Tenants
| Method | Endpoint |
|--------|----------|
| GET | `/api/v1/tenants` |
| GET | `/api/v1/tenants/{id}` |

### Documents
| Method | Endpoint |
|--------|----------|
| POST | `/api/v1/documents/upload` |
| GET | `/api/v1/documents/{id}/download` |

### Accounting
| Method | Endpoint |
|--------|----------|
| GET | `/api/v1/accounting/ledger` |
| GET | `/api/v1/accounting/trial-balance` |
| GET | `/api/v1/accounting/member-ledger/{flatId}` |

### Health
| Method | Endpoint |
|--------|----------|
| GET | `/api/v1/health` |
| GET | `/api/v1/health/db` |

### Admin/Seed
| Method | Endpoint |
|--------|----------|
| POST | `/api/v1/admin/seed` |
| GET | `/api/v1/admin/status` |

---

**Last Updated**: September 2026
