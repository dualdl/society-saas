@echo off
echo ========================================
echo   Society SaaS - Delete All Resources
echo ========================================
echo.
echo This will DELETE all Azure resources!
echo.

REM Check if Azure CLI is installed
az --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Azure CLI is not installed.
    echo Install from: https://learn.microsoft.com/en-us/cli/azure/install-azure-cli
    pause
    exit /b 1
)

REM Check if logged in
az account show >nul 2>&1
if %errorlevel% neq 0 (
    echo Not logged in. Running 'az login'...
    az login
)

REM Run the PowerShell script
powershell -ExecutionPolicy Bypass -File "%~dp0Delete-AllResources.ps1"

pause
