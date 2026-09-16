# Society SaaS - Azure Deployment Guide

## Prerequisites

- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) installed
- [GitHub CLI](https://cli.github.com/) installed
- Azure subscription
- GitHub account

---

## Step 1: Login to Azure CLI

```powershell
az login
```

Set subscription:
```powershell
az account set --subscription "YOUR_SUBSCRIPTION_ID"
```

---

## Step 2: Create Resource Group

```powershell
az group create \
  --name society-saas-rg \
  --location centralindia
```

---

## Step 3: Create Free Tier SQL Server & Database

```powershell
# Create SQL Server (Free tier)
az sql server create \
  --name society-saas-sql \
  --resource-group society-saas-rg \
  --location centralindia \
  --admin-user sqladmin \
  --admin-password Welcome@12345678

# Create Free Database (Basic 5 DTUs)
az sql db create \
  --name SocietySaaS \
  --server society-saas-sql \
  --resource-group society-saas-rg \
  --service-tier Basic \
  --capacity 5

# Allow Azure Services
az sql server firewall-rule create \
  --server society-saas-sql \
  --resource-group society-saas-rg \
  --name AllowAllWindowsAzureIps \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# Allow Local Machine IP
az sql server firewall-rule create \
  --server society-saas-sql \
  --resource-group society-saas-rg \
  --name AllowMyIP \
  --start-ip-address YOUR_PUBLIC_IP \
  --end-ip-address YOUR_PUBLIC_IP
```

---

## Step 4: Create Free Tier App Service Plan & Web Apps

```powershell
# Create Free App Service Plan (F1)
az appservice plan create \
  --name society-saas-plan \
  --resource-group society-saas-rg \
  --sku F1 \
  --is-linux

# Create API Web App
az webapp create \
  --name society-saas-api \
  --resource-group society-saas-rg \
  --plan society-saas-plan \
  --runtime "DOTNETCORE|10.0"

# Create Web App
az webapp create \
  --name society-saas-web \
  --resource-group society-saas-rg \
  --plan society-saas-plan \
  --runtime "NODE|18-lts"
```

---

## Step 5: Configure App Settings

```powershell
# API App Settings
az webapp config appsettings set \
  --name society-saas-api \
  --resource-group society-saas-rg \
  --settings \
    "AZURE_SQL_CONNECTIONSTRING=Server=tcp:society-saas-sql.database.windows.net,1433;Initial Catalog=SocietySaaS;Persist Security Info=False;User ID=sqladmin;Password=Welcome@12345678;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" \
    "JwtSettings__SecretKey=YourSuperSecretKeyHereMustBe32Characters!!" \
    "JwtSettings__Issuer=SocietySaaS" \
    "JwtSettings__Audience=SocietySaaS"

# Web App Settings
az webapp config appsettings set \
  --name society-saas-web \
  --resource-group society-saas-rg \
  --settings \
    "REACT_APP_API_URL=https://society-saas-api.azurewebsites.net"
```

---

## Step 6: Login to GitHub CLI

```powershell
gh auth login
```

---

## Step 7: Create GitHub Repository

```powershell
cd "C:\Rajesh Work\GithubCopilatSoftware\Society\society-saas"

# Create repo
gh repo create society-saas --public --source=. --remote=origin --push
```

---

## Step 8: Configure GitHub Secrets

```powershell
# Set secrets
gh secret set AZURE_WEBAPP_PUBLISH_PROFILE_API \
  --body "$(az webapp deployment list-publishing-profiles \
    --name society-saas-api \
    --resource-group society-saas-rg \
    --xml)"

gh secret set AZURE_WEBAPP_PUBLISH_PROFILE_WEB \
  --body "$(az webapp deployment list-publishing-profiles \
    --name society-saas-web \
    --resource-group society-saas-rg \
    --xml)"
```

---

## Step 9: Enable Azure AD Identity for GitHub Actions (OIDC)

```powershell
# Create Service Principal
az ad sp create-for-rbac \
  --name "society-saas-github-actions" \
  --role contributor \
  --scopes /subscriptions/YOUR_SUBSCRIPTION_ID/resourceGroups/society-saas-rg \
  --sdk-auth

# Output will give you JSON, set these as GitHub secrets:
# AZURE_CLIENT_ID
# AZURE_TENANT_ID  
# AZURE_SUBSCRIPTION_ID
```

---

## Step 10: Build & Deploy Manually (if needed)

```powershell
# Build Backend
cd backend
dotnet publish SocietySaaS.API/SocietySaaS.API.csproj \
  -c Release \
  -o ./publish

# Deploy API
az webapp deployment source config-local-git \
  --name society-saas-api \
  --resource-group society-saas-rg

az webapp deployment source config \
  --name society-saas-api \
  --resource-group society-saas-rg \
  --repo-url https://github.com/YOUR_USERNAME/society-saas.git \
  --branch master \
  --manual-integration

# Build Frontend
cd ../frontend/society-web
npm install
npm run build

# Deploy Web
az webapp deployment source config-local-git \
  --name society-saas-web \
  --resource-group society-saas-rg
```

---

## Quick Commands Summary

| Command | Description |
|---------|-------------|
| `az login` | Login to Azure |
| `az account list -o table` | List subscriptions |
| `az account set --subscription "ID"` | Set subscription |
| `az group create -n NAME -l LOCATION` | Create resource group |
| `az sql server create ...` | Create SQL Server |
| `az sql db create ...` | Create SQL Database |
| `az appservice plan create ...` | Create App Service Plan |
| `az webapp create ...` | Create Web App |
| `az webapp config appsettings set ...` | Set app settings |
| `az webapp show ...` | Show web app details |
| `az webapp browse ...` | Open in browser |
| `gh auth login` | Login to GitHub |
| `gh repo create ...` | Create GitHub repo |
| `gh secret set ...` | Set GitHub secret |
| `gh secret list` | List GitHub secrets |

---

## Cost Estimate (Free Tier)

| Resource | Tier | Cost |
|----------|------|------|
| App Service | F1 (Free) | $0/month |
| SQL Database | Basic (5 DTUs) | ~$5/month |
| Storage Account | Standard LRS | ~$0.02/GB |
| **Total** | | **~$5/month** |

---

## URLs After Deployment

- **Landing Page**: https://society-saas-web.azurewebsites.net
- **API**: https://society-saas-api.azurewebsites.net
- **Swagger**: https://society-saas-api.azurewebsites.net/swagger

---

## Troubleshooting

### Check App Logs
```powershell
az webapp log tail --name society-saas-api --resource-group society-saas-rg
```

### Restart App
```powershell
az webapp restart --name society-saas-api --resource-group society-saas-rg
```

### Delete All Resources
```powershell
az group delete --name society-saas-rg --yes --no-wait
```
