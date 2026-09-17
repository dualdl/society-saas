# Architecture Overview

## 1. Tech Stack

- **Backend**: .NET 10 Web API (Modular Monolith + Clean Architecture)
- **Frontend**: React 18+ (TypeScript + Material UI)
- **Database**: SQL Server / Azure SQL
- **Deployment**: Azure Web Apps
- **CI/CD**: GitHub Actions
- **Observability**: Application Insights

---

## 2. High-Level Architecture Diagram

```
┌─────────────────────┐
│   React UI          │
│  (Material UI)      │
└──────────┬──────────┘
           │ HTTPS
           ▼
┌─────────────────────┐
│  SocietySaaS.API    │
│  (ASP.NET Core)     │
└──────────┬──────────┘
           │
┌──────────┴──────────┐
│  Application Layer  │
│  (Business Logic)   │
└──────────┬──────────┘
           │
┌──────────┴──────────┐
│  Domain Layer       │
│  (Entities/Events)  │
└──────────┬──────────┘
           │
┌──────────┴──────────┐
│  Infrastructure     │
│  (EF Core, Azure)   │
└──────────┬──────────┘
           │
     ┌─────┴─────┐
     │           │
     ▼           ▼
┌─────────┐ ┌──────────┐
│SQL Server│ │Azure Blob│
│(Database)│ │Storage   │
└─────────┘ └──────────┘
```

---

## 3. Core Design Patterns

### Clean Architecture (Modular Monolith)

```
Domain Layer (Entities, Interfaces, Events)
    ▲
    │
Application Layer (Use Cases, DTOs, Validators, Services)
    ▲
    │
Infrastructure Layer (DB, External Services, Repositories)
    ▲
    │
API Layer (Controllers, Endpoints, Middleware)
```

### Key Patterns Used

| Pattern | Purpose | Implementation |
|---------|---------|----------------|
| **Repository** | Abstract data access | `IRepository<T>` interface |
| **Unit of Work** | Transaction management | `IUnitOfWork` service |
| **Mediator** | Command/Query handling | MediatR library |
| **Dependency Injection** | Loose coupling | Built-in .NET DI |
| **Domain Events** | Cross-module communication | Event Bus |
| **CQRS** (Optional) | Separate read/write | Per-module decision |

---

## 4. Module Boundaries

### Module: Society Management

**Responsibility**: Manage societies, buildings, flats, members

**API Endpoints**:
- `POST /api/v1/admin/societies` - Create Society
- `GET /api/v1/admin/societies` - List Societies
- `GET /api/v1/admin/societies/{id}` - Get Society
- `PUT /api/v1/admin/societies/{id}` - Update Society
- `GET /api/v1/flats` - List Flats
- `POST /api/v1/flats` - Create Flat

**Database**: Society tables in shared database

---

### Module: Billing & Payments

**Responsibility**: Generate bills, track payments, manage receipts

**API Endpoints**:
- `POST /api/v1/billing/runs` - Generate Bills
- `GET /api/v1/bills` - List Bills
- `POST /api/v1/bills/{id}/cancel` - Cancel Bill
- `POST /api/v1/payments` - Record Payment
- `GET /api/v1/payments` - List Payments
- `GET /api/v1/receipts` - List Receipts
- `GET /api/v1/receipts/{id}/pdf` - Download PDF

**Database**: Billing, Payment, Receipt tables

---

### Module: Authentication

**Responsibility**: OTP-based auth, JWT tokens, user management

**API Endpoints**:
- `POST /api/v1/auth/request-otp` - Request OTP
- `POST /api/v1/auth/verify-otp` - Verify OTP
- `POST /api/v1/auth/refresh` - Refresh Token

---

### Module: Milk & Delivery

**Responsibility**: Milk types, subscriptions, delivery boy management

**API Endpoints**:
- `GET /api/v1/milk/types` - List Milk Types
- `POST /api/v1/subscriptions` - Create Subscription
- `GET /api/v1/delivery/assignments` - List Deliveries

---

## 5. Communication Strategy

### Synchronous Communication (REST)

```
Frontend ─HTTP─> API ─HTTP─> Module Service
```

**Use Case**: Real-time data fetching, immediate response needed

**Response Format**:
```json
{
  "success": true,
  "data": { /* payload */ },
  "message": "Operation completed",
  "timestamp": "2026-09-15T10:30:00Z"
}
```

### Internal Module Communication

```
Module A ─Event─> Event Bus ─> Module B (Consumer)
```

**Use Case**: Cross-module notifications, eventual consistency

---

## 6. Data Management

### Shared Database, Separate Schemas

- All modules share one database
- Each module owns its tables
- No direct cross-module table access

**Example**:
- Society Module → `Societies`, `Buildings`, `Flats` tables
- Billing Module → `Bills`, `Payments`, `Receipts` tables
- Auth Module → `Users`, `Otps` tables

### Data Consistency

- Modules are eventually consistent via events
- Use **Saga Pattern** for distributed transactions
- Event sourcing for audit trails

---

## 7. Security Architecture

### Authentication & Authorization

```
┌──────┐
│ User │
└──┬───┘
   │ OTP / Credentials
   ▼
┌──────────────────┐
│ Auth Service     │
│ (OTP + JWT)      │
└────────┬─────────┘
         │ JWT Token
         ▼
┌──────────────────┐
│ API              │
│ (Validates JWT)  │
└────────┬─────────┘
         │ Request + Claims
         ▼
   ┌──────────────┐
   │ Module       │
   └──────────────┘
```

### Security Standards

- **OTP** for user authentication
- **JWT** for API authorization
- **Role-Based Access Control (RBAC)** via JWT claims
- **HTTPS only** for all communication
- **CORS** properly configured
- **Audit trail** for all mutations

---

## 8. Observability Stack

### Logging

```
Service → Serilog → Azure Log Analytics
                         ↓
                   (Searchable, Queryable)
```

**Log Levels**: INFO, WARNING, ERROR, DEBUG

### Monitoring & Metrics

- **Application Insights** for performance metrics
- **Custom KPIs**: API response time, error rate, throughput
- **Alerts**: Auto-scale triggers, critical failures

---

## 9. Deployment Architecture

### Azure Web Apps

```
┌─────────────────────────────────┐
│         Azure Web Apps          │
│  ┌────────────────────────────┐ │
│  │  API App Service           │ │
│  │  (society-saas-api)        │ │
│  └────────────────────────────┘ │
│  ┌────────────────────────────┐ │
│  │  Web App Service           │ │
│  │  (society-saas-web)        │ │
│  └────────────────────────────┘ │
└─────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────┐
│      Azure SQL Database         │
│      (SocietySaaS DB)           │
└─────────────────────────────────┘
```

---

## 10. Folder Structure (Backend .NET)

```
backend/
│
├── SocietySaaS.API/                 # REST Endpoints
│   ├── Controllers/
│   ├── Middleware/
│   ├── Program.cs
│   └── appsettings.json
│
├── SocietySaaS.Application/         # Use Cases & DTOs
│   ├── Commands/
│   ├── Queries/
│   ├── Handlers/
│   ├── DTOs/
│   ├── Validators/
│   └── Interfaces/
│
├── SocietySaaS.Domain/              # Core business logic
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Interfaces/
│   └── Events/
│
├── SocietySaaS.Infrastructure/      # DB, External Services
│   ├── Data/
│   ├── Repositories/
│   ├── Services/
│   └── Messaging/
│
└── SocietySaaS.Shared/              # Shared Utilities
    ├── Extensions/
    └── Helpers/
```

---

## 11. Frontend Architecture (React)

### Folder Structure

```
frontend/society-web/
│
├── src/
│   ├── features/                    # Feature-based
│   │   ├── society/
│   │   │   ├── pages/
│   │   │   ├── components/
│   │   │   ├── hooks/
│   │   │   ├── api.ts
│   │   │   └── types.ts
│   │   ├── billing/
│   │   ├── auth/
│   │   └── milk/
│   │
│   ├── shared/
│   │   ├── components/              # Reusable UI components
│   │   ├── hooks/
│   │   ├── utils/
│   │   └── types/
│   │
│   ├── services/                    # API clients
│   │   ├── api-client.ts
│   │   └── auth-service.ts
│   │
│   └── App.tsx
│
├── public/
└── package.json
```

---

## 12. Key Decisions

| Decision | Rationale |
|----------|-----------|
| **.NET 10** | Modern, performant, full-async support |
| **Modular Monolith** | Simpler deployment, shared database, easier debugging |
| **React 18+** | Large ecosystem, excellent dev experience |
| **Material UI** | Consistent, accessible, well-documented components |
| **Clean Architecture** | Testability, maintainability, domain-focus |
| **OTP Auth** | Mobile-first, no password management |

---

## 13. Future Roadmap

- [ ] Microservices extraction for billing module
- [ ] gRPC for internal communication
- [ ] CQRS for read-heavy modules
- [ ] Multi-region deployment
- [ ] GraphQL for complex queries
- [ ] Real-time notifications (SignalR)
