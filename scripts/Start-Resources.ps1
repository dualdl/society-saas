<#
.SYNOPSIS
    Starts all Azure resources for Society SaaS application.

.DESCRIPTION
    Starts App Services (API + Web) and SQL Database to resume the application.
    Use this after running Stop-Resources to bring the environment back online.

.PARAMETER ResourceGroup
    Azure Resource Group name. Defaults to "society-saas-rg".

.PARAMETER SubscriptionId
    Azure Subscription ID. If not provided, uses current subscription context.

.EXAMPLE
    .\Start-Resources.ps1
    .\Start-Resources.ps1 -ResourceGroup "society-saas-rg"
    .\Start-Resources.ps1 -ResourceGroup "society-saas-rg" -SubscriptionId "xxxx-xxxx"
#>

param(
    [string]$ResourceGroup = "society-saas-rg",
    [string]$SubscriptionId = ""
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Society SaaS - Start Azure Resources  " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
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
# Step 3: Start App Services
# -------------------------------------------
Write-Host "[3/5] Starting App Services..." -ForegroundColor Yellow

$apps = az webapp list --resource-group $ResourceGroup --query "[].{name:name, state:state}" --output json 2>$null | ConvertFrom-Json

if ($apps -and $apps.Count -gt 0) {
    foreach ($app in $apps) {
        if ($app.state -eq "Stopped" -or $app.state -eq "Stopped") {
            Write-Host "  Starting: $($app.name)..." -NoNewline
            az webapp start --name $app.name --resource-group $ResourceGroup --output none 2>$null
            if ($LASTEXITCODE -eq 0) {
                Write-Host " [STARTED]" -ForegroundColor Green
            } else {
                Write-Host " [FAILED]" -ForegroundColor Red
            }
        } else {
            Write-Host "  $($app.name): Already running ($($app.state))" -ForegroundColor Gray
        }
    }
} else {
    Write-Host "  No App Services found in resource group: $ResourceGroup" -ForegroundColor Gray
}

# -------------------------------------------
# Step 4: Start SQL Database (Resume)
# -------------------------------------------
Write-Host "[4/5] Starting SQL Database..." -ForegroundColor Yellow

$servers = az sql server list --resource-group $ResourceGroup --query "[].{name:name}" --output json 2>$null | ConvertFrom-Json

if ($servers -and $servers.Count -gt 0) {
    foreach ($server in $servers) {
        $databases = az sql db list --server $server.name --resource-group $ResourceGroup --query "[?name!='master'].{name:name,state:status}" --output json 2>$null | ConvertFrom-Json

        if ($databases -and $databases.Count -gt 0) {
            foreach ($db in $databases) {
                if ($db.state -eq "Paused") {
                    Write-Host "  Resuming database: $($db.name) on $($server.name)..." -NoNewline
                    az sql db resume --name $db.name --server $server.name --resource-group $ResourceGroup --output none 2>$null
                    if ($LASTEXITCODE -eq 0) {
                        Write-Host " [RESUMED]" -ForegroundColor Green
                    } else {
                        Write-Host " [FAILED]" -ForegroundColor Red
                    }
                } else {
                    Write-Host "  $($db.name): Already online ($($db.state))" -ForegroundColor Gray
                }
            }
        }
    }
} else {
    Write-Host "  No SQL Servers found in resource group: $ResourceGroup" -ForegroundColor Gray
}

# -------------------------------------------
# Step 5: Summary
# -------------------------------------------
Write-Host ""
Write-Host "[5/5] Verifying resource status..." -ForegroundColor Yellow

Write-Host ""
Write-Host "----------------------------------------" -ForegroundColor Cyan
Write-Host "  App Services:" -ForegroundColor Cyan
az webapp list --resource-group $ResourceGroup --query "[].{Name:name, State:state}" --output table 2>$null

Write-Host ""
Write-Host "  SQL Databases:" -ForegroundColor Cyan
$servers = az sql server list --resource-group $ResourceGroup --query "[].name" --output tsv 2>$null
foreach ($server in $servers) {
    az sql db list --server $server --resource-group $ResourceGroup --query "[?name!='master'].{Name:name, State:status}" --output table 2>$null
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  All resources STARTED successfully!   " -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "  API URL:   https://$(($apps | Where-Object { $_.name -like '*api*' }).name).azurewebsites.net" -ForegroundColor White
Write-Host "  Web URL:   https://$(($apps | Where-Object { $_.name -like '*web*' }).name).azurewebsites.net" -ForegroundColor White
Write-Host "  Swagger:   https://$(($apps | Where-Object { $_.name -like '*api*' }).name).azurewebsites.net/swagger" -ForegroundColor White
Write-Host ""
