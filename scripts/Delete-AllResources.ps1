<#
.SYNOPSIS
    Deletes ALL Azure resources for the Society SaaS project.

.DESCRIPTION
    Deletes the entire resource group and all resources within it.
    This is DESTRUCTIVE and IRREVERSIBLE.

.PARAMETER ResourceGroup
    Azure Resource Group name. Defaults to "society-saas-rg".

.PARAMETER SkipConfirmation
    Skip the confirmation prompt (useful for CI/CD).

.EXAMPLE
    .\Delete-AllResources.ps1
    .\Delete-AllResources.ps1 -SkipConfirmation
#>

param(
    [string]$ResourceGroup = "society-saas-rg",
    [switch]$SkipConfirmation
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Red
Write-Host "  Society SaaS - DELETE ALL RESOURCES   " -ForegroundColor Red
Write-Host "========================================" -ForegroundColor Red
Write-Host ""
Write-Host "  Resource Group: $ResourceGroup" -ForegroundColor Yellow
Write-Host ""

# -------------------------------------------
# Step 1: List all resources that will be deleted
# -------------------------------------------
Write-Host "-------------------------------------------" -ForegroundColor Gray
Write-Host "  Resources that WILL BE DELETED:" -ForegroundColor Red
Write-Host "-------------------------------------------" -ForegroundColor Gray

$resources = az resource list --resource-group $ResourceGroup --query "[].{Name:name, Type:type, Location:location}" --output json 2>$null | ConvertFrom-Json

if (-not $resources -or $resources.Count -eq 0) {
    Write-Host "  No resources found in '$ResourceGroup'." -ForegroundColor Gray
    Write-Host "  Resource group may already be deleted." -ForegroundColor Gray
    exit 0
}

foreach ($r in $resources) {
    Write-Host "  - $($r.Name) ($($r.Type))" -ForegroundColor Red
}

Write-Host ""
Write-Host "  Total resources: $($resources.Count)" -ForegroundColor Yellow
Write-Host ""

# -------------------------------------------
# Step 2: Confirmation
# -------------------------------------------
if (-not $SkipConfirmation) {
    Write-Host "  WARNING: This will PERMANENTLY DELETE all resources above!" -ForegroundColor Red
    Write-Host "  This action CANNOT be undone." -ForegroundColor Red
    Write-Host ""
    $confirm = Read-Host "  Type 'DELETE' to confirm"
    
    if ($confirm -ne "DELETE") {
        Write-Host ""
        Write-Host "  Aborted. No resources were deleted." -ForegroundColor Gray
        exit 0
    }
}

# -------------------------------------------
# Step 3: Delete the resource group
# -------------------------------------------
Write-Host ""
Write-Host "  Deleting resource group '$ResourceGroup'..." -ForegroundColor Red
Write-Host "  This may take 1-3 minutes..." -ForegroundColor Yellow
Write-Host ""

try {
    az group delete --name $ResourceGroup --yes --output none
    Write-Host "  Resource group '$ResourceGroup' deleted successfully." -ForegroundColor Green
} catch {
    Write-Host "  Error deleting resource group: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# -------------------------------------------
# Step 4: Verify deletion
# -------------------------------------------
Write-Host ""
Write-Host "  Verifying deletion..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

$check = az group exists --name $ResourceGroup --output tsv 2>$null
if ($check -eq "false") {
    Write-Host "  Verified: Resource group '$ResourceGroup' no longer exists." -ForegroundColor Green
} else {
    Write-Host "  Resource group still exists (may be deleting in background)." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Done! All resources deleted." -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "  To recreate resources, run:" -ForegroundColor Cyan
Write-Host "  .\Deploy-FullInfrastructure.ps1" -ForegroundColor White
Write-Host ""
