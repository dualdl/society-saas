<#
.SYNOPSIS
    Stops all Azure resources for Society SaaS application to save costs.

.DESCRIPTION
    Stops App Services (API + Web) and pauses SQL Database.
    Use this when the environment is not needed (e.g., nights, weekends).
    Run Start-Resources.ps1 to bring everything back online.

.PARAMETER ResourceGroup
    Azure Resource Group name. Defaults to "society-saas-rg".

.PARAMETER SubscriptionId
    Azure Subscription ID. If not provided, uses current subscription context.

.PARAMETER Force
    Skip confirmation prompt.

.EXAMPLE
    .\Stop-Resources.ps1
    .\Stop-Resources.ps1 -ResourceGroup "society-saas-rg"
    .\Stop-Resources.ps1 -Force
#>

param(
    [string]$ResourceGroup = "society-saas-rg",
    [string]$SubscriptionId = "",
    [switch]$Force
)

$ErrorActionPreference = "Stop"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "  Society SaaS - Stop Azure Resources     " -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# -------------------------------------------
# Step 1: Validate Azure CLI & Login
# -------------------------------------------
Write-Host "[1/5] Checking Azure CLI..." -ForegroundColor Yellow

try {
    $azVersion = az version --output json 2>$null | ConvertFrom-Json
    Write-Host "  Azure CLI version: $($azVersion.'azure-cli')" -ForegroundColor Green
} catch {
    Write-Host "  ERROR: Azure CLI not installed. Install from https://aka.ms/azure-cli" -ForegroundColor Red
    exit 1
}

$account = az account show --output json 2>$null | ConvertFrom-Json
if (-not $account) {
    Write-Host "  Not logged in. Running az login..." -ForegroundColor Yellow
    az login --output none
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  ERROR: Azure login failed." -ForegroundColor Red
        exit 1
    }
}

Write-Host "  Logged in as: $($account.user.name)" -ForegroundColor Green

# -------------------------------------------
# Step 2: Set Subscription (if provided)
# -------------------------------------------
Write-Host "[2/5] Setting subscription..." -ForegroundColor Yellow

if ($SubscriptionId) {
    az account set --subscription $SubscriptionId --output none
    Write-Host "  Subscription set to: $SubscriptionId" -ForegroundColor Green
} else {
    Write-Host "  Using current subscription: $($account.id)" -ForegroundColor Green
}

# -------------------------------------------
# Step 3: Confirmation Prompt
# -------------------------------------------
if (-not $Force) {
    Write-Host ""
    Write-Host "  WARNING: This will STOP the following resources:" -ForegroundColor Yellow
    Write-Host "    - App Services (API + Web)" -ForegroundColor Yellow
    Write-Host "    - SQL Database (will be paused)" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "  The application will be UNAVAILABLE until you run Start-Resources.ps1" -ForegroundColor Yellow
    Write-Host ""

    $confirm = Read-Host "  Do you want to continue? (y/N)"
    if ($confirm -ne 'y' -and $confirm -ne 'Y') {
        Write-Host "  Operation cancelled." -ForegroundColor Gray
        exit 0
    }
}

# -------------------------------------------
# Step 4: Stop App Services
# -------------------------------------------
Write-Host ""
Write-Host "[3/5] Stopping App Services..." -ForegroundColor Yellow

$apps = az webapp list --resource-group $ResourceGroup --query "[].{name:name, state:state}" --output json 2>$null | ConvertFrom-Json

if ($apps -and $apps.Count -gt 0) {
    foreach ($app in $apps) {
        if ($app.state -eq "Running") {
            Write-Host "  Stopping: $($app.name)..." -NoNewline
            az webapp stop --name $app.name --resource-group $ResourceGroup --output none 2>$null
            if ($LASTEXITCODE -eq 0) {
                Write-Host " [STOPPED]" -ForegroundColor Yellow
            } else {
                Write-Host " [FAILED]" -ForegroundColor Red
            }
        } else {
            Write-Host "  $($app.name): Already stopped ($($app.state))" -ForegroundColor Gray
        }
    }
} else {
    Write-Host "  No App Services found in resource group: $ResourceGroup" -ForegroundColor Gray
}

# -------------------------------------------
# Step 5: Pause SQL Database
# -------------------------------------------
Write-Host "[4/5] Pausing SQL Database..." -ForegroundColor Yellow

$servers = az sql server list --resource-group $ResourceGroup --query "[].{name:name}" --output json 2>$null | ConvertFrom-Json

if ($servers -and $servers.Count -gt 0) {
    foreach ($server in $servers) {
        $databases = az sql db list --server $server.name --resource-group $ResourceGroup --query "[?name!='master'].{name:name,state:status}" --output json 2>$null | ConvertFrom-Json

        if ($databases -and $databases.Count -gt 0) {
            foreach ($db in $databases) {
                if ($db.state -eq "Online") {
                    Write-Host "  Pausing database: $($db.name) on $($server.name)..." -NoNewline
                    az sql db stop --name $db.name --server $server.name --resource-group $ResourceGroup --output none 2>$null
                    if ($LASTEXITCODE -eq 0) {
                        Write-Host " [PAUSED]" -ForegroundColor Yellow
                    } else {
                        Write-Host " [FAILED]" -ForegroundColor Red
                    }
                } else {
                    Write-Host "  $($db.name): Already paused ($($db.state))" -ForegroundColor Gray
                }
            }
        }
    }
} else {
    Write-Host "  No SQL Servers found in resource group: $ResourceGroup" -ForegroundColor Gray
}

# -------------------------------------------
# Step 6: Summary
# -------------------------------------------
Write-Host ""
Write-Host "[5/5] Verifying resource status..." -ForegroundColor Yellow

Write-Host ""
Write-Host "----------------------------------------" -ForegroundColor Cyan
Write-Host "  App Services:" -ForegroundColor Cyan
az webapp list --resource-group $ResourceGroup --query "[].{Name:name, State:state}" --output table 2>$null

Write-Host ""
Write-Host "  SQL Databases:" -ForegroundColor Cyan
foreach ($server in $servers) {
    az sql db list --server $server.name --resource-group $ResourceGroup --query "[?name!='master'].{Name:name, State:status}" --output table 2>$null
}

# Calculate estimated savings
$appCount = if ($apps) { $apps.Count } else { 0 }
Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "  All resources STOPPED successfully!     " -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""
Write-Host "  Estimated savings:" -ForegroundColor White
Write-Host "    - $appCount App Service(s) stopped" -ForegroundColor White
Write-Host "    - SQL Database paused" -ForegroundColor White
Write-Host "    - Approx ~$0.02/hour saved" -ForegroundColor White
Write-Host ""
Write-Host "  To bring resources back online:" -ForegroundColor Yellow
Write-Host "    .\Start-Resources.ps1" -ForegroundColor White
Write-Host ""
