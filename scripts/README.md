# Azure Resource Management Scripts

PowerShell scripts to start/stop Azure resources for cost savings.

## Scripts

| Script | Purpose |
|--------|---------|
| `Start-Resources.ps1` | Start all Azure resources |
| `Stop-Resources.ps1` | Stop all Azure resources |
| `Get-ResourceStatus.ps1` | Show current resource status |

## Usage

### Stop Resources (Save Cost)
```powershell
.\scripts\Stop-Resources.ps1
```

### Start Resources
```powershell
.\scripts\Start-Resources.ps1
```

### Check Status
```powershell
.\scripts\Get-ResourceStatus.ps1
```

## Parameters

### Start-Resources.ps1
| Parameter | Default | Description |
|-----------|---------|-------------|
| `-ResourceGroup` | `society-saas-rg` | Azure Resource Group name |
| `-SubscriptionId` | (current) | Azure Subscription ID |

### Stop-Resources.ps1
| Parameter | Default | Description |
|-----------|---------|-------------|
| `-ResourceGroup` | `society-saas-rg` | Azure Resource Group name |
| `-SubscriptionId` | (current) | Azure Subscription ID |
| `-Force` | `false` | Skip confirmation prompt |

## Examples

```powershell
# Stop with confirmation
.\scripts\Stop-Resources.ps1

# Stop without confirmation
.\scripts\Stop-Resources.ps1 -Force

# Start specific resource group
.\scripts\Start-Resources.ps1 -ResourceGroup "society-saas-prod-rg"

# Start with specific subscription
.\scripts\Start-Resources.ps1 -SubscriptionId "xxxx-xxxx-xxxx"
```

## Prerequisites

1. **Azure CLI** installed - [Install](https://aka.ms/azure-cli)
2. Logged in: `az login`
3. Correct subscription selected: `az account set --subscription "ID"`

## What Gets Stopped

| Resource | Action | Savings |
|----------|--------|---------|
| App Service (API) | Stopped | ~$0.02/hr |
| App Service (Web) | Stopped | ~$0.02/hr |
| SQL Database | Paused | ~$0.01/hr |

## Cost Savings Example

If you stop resources for 16 hours/day (overnight + weekend):
- **Daily savings**: ~$0.50
- **Monthly savings**: ~$15

## Troubleshooting

### "Not logged in" error
```powershell
az login
```

### "Resource group not found"
```powershell
az group list --query "[].name" --output table
```

### SQL Database won't pause
SQL Database can only be paused if it's in **Serverless** tier. Check:
```powershell
az sql db show --name "SocietySaaS" --server "your-server" --resource-group "society-saas-rg" --query "sku.tier"
```

If not serverless, you can still stop App Services (which saves the most cost).
