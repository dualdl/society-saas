# Complete Repository Setup Guide

**Status**: COMPLETE - Full-stack Society SaaS platform with documentation and CI/CD

---

## What's Included

### Backend (.NET 10, Clean Architecture)

**Location**: `backend/`

- **SocietySaaS.API**: ASP.NET Core Web API with controllers, middleware
- **SocietySaaS.Application**: Business logic, DTOs, validators, services
- **SocietySaaS.Domain**: Core entities, interfaces, events
- **SocietySaaS.Infrastructure**: Data access, repositories, EF Core
- **SocietySaaS.Shared**: Shared utilities and extensions

**Features**:
- Entity Framework Core with migrations
- Repository + Unit of Work pattern
- Dependency Injection
- Structured logging (Serilog)
- Swagger/OpenAPI documentation
- JWT/OTP Authentication
- Unit tests with Moq

### Frontend (React 18+ with TypeScript)

**Location**: `frontend/society-web/`

- React functional components with hooks
- TypeScript strict mode
- Material UI components
- Centralized API client
- Custom hooks for async operations
- Error boundaries

### Infrastructure (Azure Bicep)

**Location**: `infra/`

- Azure Web Apps
- Azure SQL Database
- Azure Blob Storage
- Application Insights

### CI/CD (GitHub Actions)

**Location**: `.github/workflows/`

- PR validation workflow
- Staging deployment workflow
- Production deployment workflow

---

## How to Use This Repository

### For Building New Features

```bash
# 1. Create feature branch
git checkout develop
git checkout -b feature/your-feature

# 2. Make changes following coding standards
# See: docs/architecture/coding-standards.md

# 3. Write tests
# See: docs/architecture/testing.md

# 4. Create PR
# See: docs/architecture/branching-strategy.md
```

### For Backend Development

```bash
cd backend
dotnet restore
dotnet build
dotnet run --project SocietySaaS.API
```

### For Frontend Development

```bash
cd frontend/society-web
npm install
npm start
```

### For Deployment

```bash
# Using Azure CLI
az group create --name society-saas-rg --location centralindia
az deployment group create --resource-group society-saas-rg --template-file infra/main.bicep

# Using azd
azd up
```

---

## Key Documentation Files

| File | Purpose |
|------|---------|
| `docs/architecture/architecture.md` | System design overview |
| `docs/architecture/coding-standards.md` | Code styling rules |
| `docs/api/api-guidelines.md` | REST API standards |
| `docs/architecture/code-review.md` | PR review process |
| `docs/architecture/testing.md` | Testing strategy |
| `docs/architecture/ci-cd.md` | CI/CD pipeline |
| `docs/architecture/branching-strategy.md` | Git Flow model |
| `docs/architecture/troubleshooting.md` | Common issues |
| `AGENTS.md` | Available agents |
| `NEW_SERVICE_GUIDE.md` | How to build new modules |

---

## Architecture Overview

```
+-----------------------------------------------------------+
|                  Frontend (React 18+)                      |
|  - TypeScript strict mode                                 |
|  - Material UI components                                 |
|  - Axios API client with interceptors                     |
|  - Custom hooks for async state                           |
+------------+---------------------------+------------------+
             |                           |
             +------------+--------------+
         API Gateway / Load Balancer
             |
   +---------+----------+--------------+
   |                    |              |
   v                    v              v
+----------+     +----------+    +----------+
| Society  |     | Billing  |    | Milk     |
| Module   |     | Module   |    | Module   |
+----------+     +----------+    +----------+
   |                    |              |
   +--------------------+--------------+
                      |
         +------------+-----------+
         |                        |
         v                        v
    +-----------+          +-----------+
    | SQL Server|          | Azure Blob|
    | (Database)|          | Storage   |
    +-----------+          +-----------+
```

---

## Key Features

### Backend
- Clean Architecture (domain -> application -> infrastructure -> API)
- Entity Framework Core with migrations
- Repository pattern for data access
- Dependency injection
- Structured logging (Serilog)
- Error handling with standardized responses
- Swagger/OpenAPI documentation
- CORS configuration
- Unit tests with Moq
- Soft delete audit trail
- Multi-tenant support

### Frontend
- React functional components with hooks
- TypeScript strict mode
- Material UI components
- Centralized API client with interceptors
- Custom hooks for async operations
- Error boundaries
- Responsive CSS

### Infrastructure
- Azure Web Apps hosting
- Azure SQL Database
- Azure Blob Storage
- Application Insights
- Managed identity for secure access
- Tagging for cost tracking

---

## Next Steps

### 1. Start Building
- Use existing modules as reference
- Follow coding standards
- Write tests for new features

### 2. Deploy to Azure
- Create resource group
- Deploy infrastructure using Bicep
- Configure GitHub Secrets
- Push to main branch

### 3. Follow AI Framework
- Use `.github/copilot-instructions.md` for coding standards
- Create new agents for project-specific needs
- Reference AGENTS.md for available agents

---

## Learning Resources

**Backend (.NET Clean Architecture):**
- [Microsoft: Clean Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Best Practices](https://learn.microsoft.com/en-us/aspnet/core/)

**Frontend (React + TypeScript):**
- [React Docs](https://react.dev)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)
- [Material UI Docs](https://mui.com/material-ui/)

**Infrastructure (Azure):**
- [Azure Bicep Documentation](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/)
- [Azure Web Apps](https://learn.microsoft.com/en-us/azure/app-service/)

---

**Repository Version**: 1.0
**Last Updated**: September 2026
**Status**: Production Ready
**Framework**: .NET 10, React 18+, Bicep
**License**: MIT
