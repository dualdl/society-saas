# Society Management SaaS

A comprehensive, multi-tenant society management platform built with **.NET 10**, **React + TypeScript**, **Material UI**, and **Azure**.

![Status](https://img.shields.io/badge/status-active-success) ![License](https://img.shields.io/badge/license-MIT-blue) ![.NET](https://img.shields.io/badge/.NET-10.0-512bd4) ![React](https://img.shields.io/badge/React-18%2B-61dafb)

---

## Quick Navigation

**Documentation** (Start here!):
- [Architecture Overview](docs/architecture/architecture.md) - System design & patterns
- [Coding Standards](docs/architecture/coding-standards.md) - Code style & conventions
- [API Guidelines](docs/api/api-guidelines.md) - REST API standards
- [Code Review Guidelines](docs/architecture/code-review.md) - Review process
- [Testing Strategy](docs/architecture/testing.md) - Unit, integration, E2E tests
- [CI/CD Pipeline](docs/architecture/ci-cd.md) - GitHub Actions & deployment
- [Branching Strategy](docs/architecture/branching-strategy.md) - Git Flow model
- [Troubleshooting](docs/architecture/troubleshooting.md) - Common issues & solutions

**AI & Automation**:
- [Agent Guidelines](AGENTS.md) - Specialized agent personas
- [Copilot Instructions](.github/copilot-instructions.md) - How agents work with this repo

**Deployment**:
- [Azure Deployment Guide](docs/DEPLOYMENT.md) - Azure setup & deployment

---

## Tech Stack

| Layer           | Technology                                         |
| --------------- | -------------------------------------------------- |
| Backend         | C# / .NET 10                                       |
| API             | ASP.NET Core Web API                               |
| Architecture    | Modular Monolith + Clean Architecture              |
| ORM             | Entity Framework Core                              |
| Database        | SQL Server / Azure SQL                             |
| Frontend        | React 18+ + TypeScript                             |
| UI              | Material UI                                        |
| Excel           | ClosedXML                                          |
| PDF             | QuestPDF                                           |
| Authentication  | Mobile/Email OTP                                   |
| Email           | SMTP / Azure Communication Services                |
| Storage         | Azure Blob Storage                                 |
| Hosting         | Azure Web Apps                                     |
| CI/CD           | GitHub Actions                                     |

---

## Project Structure

```
society-saas/
│
├── backend/
│   ├── SocietySaaS.API           # ASP.NET Core Web API
│   ├── SocietySaaS.Application   # Business Logic Layer
│   ├── SocietySaaS.Domain        # Domain Entities
│   ├── SocietySaaS.Infrastructure# Data Access & Services
│   └── SocietySaaS.Shared        # Shared Utilities
│
├── frontend/
│   └── society-web               # React + TypeScript + Material UI
│
├── infra/                        # Azure Bicep Templates
│
├── docs/                         # Documentation
│   ├── architecture/             # Architecture & coding docs
│   ├── api/                      # API guidelines
│   └── database/                 # Database docs
│
├── database/
│   ├── migrations                # EF Core Migrations
│   ├── scripts                   # SQL Scripts
│   └── seed                      # Seed Data
│
├── tests/                        # Test projects
│
├── .github/
│   └── workflows                 # CI/CD Pipelines
│
└── azure.yaml                    # Azure Developer CLI config
```

---

## Getting Started

### Prerequisites

- **.NET 10 SDK** - [Download](https://dotnet.microsoft.com/download)
- **Node.js 18+** - [Download](https://nodejs.org)
- **SQL Server** (LocalDB or Azure SQL)

### Backend Setup

```bash
cd backend
dotnet restore
dotnet build
dotnet run --project SocietySaaS.API
```

### Frontend Setup

```bash
cd frontend/society-web
npm install
npm start
```

### Database Setup

```bash
cd backend
dotnet ef migrations add InitialCreate --project SocietySaaS.Infrastructure --startup-project SocietySaaS.API
dotnet ef database update --project SocietySaaS.Infrastructure --startup-project SocietySaaS.API
```

---

## API Endpoints

### Authentication
- `POST /api/v1/auth/request-otp` - Request OTP
- `POST /api/v1/auth/verify-otp` - Verify OTP
- `POST /api/v1/auth/refresh` - Refresh Token

### Society Management
- `POST /api/v1/admin/societies` - Create Society
- `GET /api/v1/admin/societies` - List Societies
- `GET /api/v1/admin/societies/{id}` - Get Society

### Flat Management
- `GET /api/v1/flats` - List Flats
- `POST /api/v1/flats` - Create Flat
- `PUT /api/v1/flats/{id}` - Update Flat

### Billing
- `POST /api/v1/billing/runs` - Generate Bills
- `GET /api/v1/bills` - List Bills
- `POST /api/v1/bills/{id}/cancel` - Cancel Bill

### Payments
- `POST /api/v1/payments` - Record Payment
- `GET /api/v1/payments` - List Payments
- `POST /api/v1/payments/{id}/reverse` - Reverse Payment

### Receipts
- `GET /api/v1/receipts` - List Receipts
- `GET /api/v1/receipts/{id}/pdf` - Download PDF
- `POST /api/v1/receipts/{id}/email` - Email Receipt

---

## Deployment

### Azure Deployment

1. Create Azure resources using Bicep templates:
   ```bash
   az deployment group create \
     --resource-group society-saas-rg \
     --template-file infra/main.bicep \
     --parameters environmentName=dev sqlAdminLogin=admin sqlAdminPassword=password
   ```

2. Configure GitHub Secrets for CI/CD

3. Push to main branch to trigger deployment

### Environment Variables

| Variable | Description |
|----------|-------------|
| `AZURE_SQL_CONNECTIONSTRING` | SQL Server connection string |
| `JwtSettings:SecretKey` | JWT signing key |
| `AzureStorage:ConnectionString` | Azure Storage connection |
| `EmailSettings:SmtpHost` | SMTP server host |

---

## Features

- Multi-tenant architecture
- Role-based access control (RBAC)
- Smart billing engine
- Payment tracking
- Receipt generation (PDF)
- Excel import/export
- Audit trail
- Email notifications
- Mobile-responsive UI
- CSV onboarding for tenant creation

---

## Standards & Practices

### Coding
- Clean Architecture for all modules
- SOLID principles mandatory
- Repository + Unit of Work pattern
- Dependency Injection everywhere
- Async/await for I/O operations

### Code Review
- Max 400 lines per PR
- 1+ approval required
- All CI/CD checks must pass
- 80%+ code coverage

### Testing
- 70% unit tests, 20% integration, 10% E2E
- Minimum 80% code coverage
- All tests pass before merge

### Branching
- `main` - production (protected)
- `develop` - staging (protected)
- `feature/*` - feature branches
- `hotfix/*` - emergency fixes
- PR required for all changes

---

## License

MIT

---

**Last Updated**: September 2026
**Team**: Society SaaS Engineering
**Version**: 1.0.0
