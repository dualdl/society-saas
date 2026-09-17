<#
.SYNOPSIS
    Shows status of all Society SaaS Azure resources.

.DESCRIPTION
    Displays the current state of App Services, SQL Database, and other resources.

.PARAMETER ResourceGroup
    Azure Resource Group name. Defaults to "society-saas-rg".

.EXAMPLE
    .\Get-ResourceStatus.ps1
    .\Get-ResourceStatus.ps1 -ResourceGroup "society-saas-prod-rg"
#>

param(
    [string]$ResourceGroup = "society-saas-rg"
)

$ErrorActionPreference = "SilentlyContinue"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Society SaaS - Resource Status         " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Resource Group: $ResourceGroup" -ForegroundColor White
Write-Host ""

# -------------------------------------------
# App Services
# -------------------------------------------
Write-Host "-------------------------------------------" -ForegroundColor Gray
Write-Host "  App Services:" -ForegroundColor Yellow
Write-Host "-------------------------------------------" -ForegroundColor Gray

$apps = az webapp list --resource-group $ResourceGroup --query "[].{Name:name, State:state, Plan:appServicePlanId}" --output json 2>$null | ConvertFrom-Json

if ($apps -and $apps.Count -gt 0) {
    foreach ($app in $apps) {
        $stateColor = if ($app.State -eq "Running") { "Green" } elseif ($app.State -eq "Stopped") { "Red" } else { "Yellow" }
        Write-Host "  $($app.Name): " -NoNewline -ForegroundColor White
        Write-Host "$($app.State)" -ForegroundColor $stateColor
    }
} else {
    Write-Host "  No App Services found" -ForegroundColor Gray
}

# -------------------------------------------
# SQL Database
# -------------------------------------------
Write-Host ""
Write-Host "-------------------------------------------" -ForegroundColor Gray
Write-Host "  SQL Database:" -ForegroundColor Yellow
Write-Host "-------------------------------------------" -ForegroundColor Gray

$servers = az sql server list --resource-group $ResourceGroup --query "[].{name:name, fqdn:fullyQualifiedDomainName}" --output json 2>$null | ConvertFrom-Json

if ($servers -and $servers.Count -gt 0) {
    foreach ($server in $servers) {
        Write-Host "  Server: $($server.fqdn)" -ForegroundColor White

        $databases = az sql db list --server $server.name --resource-group $ResourceGroup --query "[?name!='master'].{Name:name, State:status, Edition:edition, Tier:serviceObjective}" --output json 2>$null | ConvertFrom-Json

        if ($databases -and $databases.Count -gt 0) {
            foreach ($db in $databases) {
                $stateColor = if ($db.State -eq "Online") { "Green" } elseif ($db.State -eq "Paused") { "Red" } else { "Yellow" }
                Write-Host "    $($db.Name): " -NoNewline -ForegroundColor White
                Write-Host "$($db.State)" -NoNewline -ForegroundColor $stateColor
                Write-Host " | $($db.Edition) | $($db.Tier)" -ForegroundColor Gray
            }
        } else {
            Write-Host "    No databases found" -ForegroundColor Gray
        }
    }
} else {
    Write-Host "  No SQL Servers found" -ForegroundColor Gray
}

# -------------------------------------------
# Application Insights
# -------------------------------------------
Write-Host ""
Write-Host "-------------------------------------------" -ForegroundColor Gray
Write-Host "  Application Insights:" -ForegroundColor Yellow
Write-Host "-------------------------------------------" -ForegroundColor Gray

$components = az monitor app-insights component list --resource-group $ResourceGroup --query "[].{Name:name, AppId:appId}" --output json 2>$null | ConvertFrom-Json

if ($components -and $components.Count -gt 0) {
    foreach ($component in $components) {
        Write-Host "  $($component.Name): Active" -ForegroundColor Green
    }
} else {
    Write-Host "  No Application Insights found" -ForegroundColor Gray
}

# -------------------------------------------
# Storage Account
# -------------------------------------------
Write-Host ""
Write-Host "-------------------------------------------" -ForegroundColor Gray
Write-Host "  Storage Account:" -ForegroundColor Yellow
Write-Host "-------------------------------------------" -ForegroundColor Gray

$storageAccounts = az storage account list --resource-group $ResourceGroup --query "[].{Name:name, Status:statusOfPrimary}" --output json 2>$null | ConvertFrom-Json

if ($storageAccounts -and $storageAccounts.Count -gt 0) {
    foreach ($storage in $storageAccounts) {
        Write-Host "  $($storage.Name): $($storage.Status)" -ForegroundColor Green
    }
} else {
    Write-Host "  No Storage Accounts found" -ForegroundColor Gray
}

# -------------------------------------------
# Key Vault
# -------------------------------------------
Write-Host ""
Write-Host "-------------------------------------------" -ForegroundColor Gray
Write-Host "  Key Vault:" -ForegroundColor Yellow
Write-Host "-------------------------------------------" -ForegroundColor Gray

$vaults = az keyvault list --resource-group $ResourceGroup --query "[].{Name:name}" --output json 2>$null | ConvertFrom-Json

if ($vaults -and $vaults.Count -gt 0) {
    foreach ($vault in $vaults) {
        Write-Host "  $($vault.Name): Active" -ForegroundColor Green
    }
} else {
    Write-Host "  No Key Vaults found" -ForegroundColor Gray
}

# -------------------------------------------
# Summary
# -------------------------------------------
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Commands:" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Start all: .\Start-Resources.ps1" -ForegroundColor White
Write-Host "  Stop all:  .\Stop-Resources.ps1" -ForegroundColor White
Write-Host ""
