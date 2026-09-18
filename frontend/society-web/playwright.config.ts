import { defineConfig, devices } from '@playwright/test';
import dotenv from 'dotenv';
import path from 'path';

dotenv.config({ path: path.resolve(__dirname, '.env.test') });

export default defineConfig({
  testDir: './tests',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: [
    ['html', { open: 'never' }],
    ['list'],
    ...(process.env.CI ? [['github' as const]] : []),
  ],
  use: {
    baseURL: process.env.BASE_URL || 'https://society-saas-web.azurewebsites.net',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    actionTimeout: 10000,
    navigationTimeout: 30000,
  },
  projects: [
    {
      name: 'setup',
      testMatch: /auth\.setup\.ts$/,
    },
    {
      name: 'chromium',
      use: {
        ...devices['Desktop Chrome'],
        storageState: './tests/.auth/society-admin.json',
      },
      dependencies: ['setup'],
    },
    {
      name: 'firefox',
      use: {
        ...devices['Desktop Firefox'],
        storageState: './tests/.auth/society-admin.json',
      },
      dependencies: ['setup'],
    },
    {
      name: 'webkit',
      use: {
        ...devices['Desktop Safari'],
        storageState: './tests/.auth/society-admin.json',
      },
      dependencies: ['setup'],
    },
    {
      name: 'Mobile Chrome',
      use: {
        ...devices['Pixel 5'],
        storageState: './tests/.auth/society-admin.json',
      },
      dependencies: ['setup'],
    },
    {
      name: 'Mobile Safari',
      use: {
        ...devices['iPhone 12'],
        storageState: './tests/.auth/society-admin.json',
      },
      dependencies: ['setup'],
    },
  ],
});
