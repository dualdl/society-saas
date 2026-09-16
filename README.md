# Society Management SaaS

A comprehensive society management platform built with modern technologies.

## Tech Stack

| Layer           | Technology                                         |
| --------------- | -------------------------------------------------- |
| Backend         | C#                                                 |
| API             | ASP.NET Core Web API                               |
| Architecture    | Modular Monolith + Clean Architecture              |
| ORM             | Entity Framework Core                              |
| Database        | SQL Server / Azure SQL                             |
| Frontend        | React + TypeScript                                 |
| UI              | Material UI                                        |
| Excel           | ClosedXML                                          |
| PDF             | QuestPDF                                           |
| Authentication  | Mobile/Email OTP                                   |
| Email           | SMTP / Azure Communication Services                |
| Storage         | Azure Blob Storage                                 |
| Hosting         | Azure Web Apps                                     |
| CI/CD           | GitHub Actions                                     |

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
│   └── society-web               # React + TypeScript
│
├── infra/                        # Azure Bicep Templates
│
├── .github/
│   └── workflows                 # CI/CD Pipelines
│
└── database/
    ├── migrations                # EF Core Migrations
    ├── scripts                   # SQL Scripts
    └── seed                      # Seed Data
```

## Getting Started

### Prerequisites

- .NET 10 SDK
- Node.js 18+
- SQL Server (LocalDB or Azure SQL)

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

## License

MIT
