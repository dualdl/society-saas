# New Module Development Guide

Use this guide when building a new module in the Society SaaS platform. This document tells agents exactly what needs to be done.

---

## 1. Pre-Development Checklist

Before starting, confirm:
- [ ] Module name decided and approved
- [ ] Business requirements documented
- [ ] Database schema planned
- [ ] API endpoints defined
- [ ] Team assigned

---

## 2. Project Setup (Copy & Customize)

### A. Identify Module Location

The Society SaaS is a **Modular Monolith**. New modules go in the existing backend projects:

```
backend/
├── SocietySaaS.Domain/        # Add entities here
├── SocietySaaS.Application/   # Add services, DTOs, validators here
├── SocietySaaS.Infrastructure/# Add repositories, DbContext here
└── SocietySaaS.API/           # Add controllers here
```

### B. Create Domain Entities

Create in `SocietySaaS.Domain/Entities/`:

```csharp
namespace SocietySaaS.Domain.Entities;

public class YourEntity : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    // Add your properties
}
```

### C. Create Repository Interface

Create in `SocietySaaS.Domain/Interfaces/`:

```csharp
namespace SocietySaaS.Domain.Interfaces;

public interface IYourEntityRepository : IRepository<YourEntity>
{
    Task<YourEntity?> GetByNameAsync(string name);
    Task<IReadOnlyList<YourEntity>> GetAllActiveAsync();
}
```

### D. Create Repository Implementation

Create in `SocietySaaS.Infrastructure/Repositories/`:

```csharp
namespace SocietySaaS.Infrastructure.Repositories;

public class YourEntityRepository : Repository<YourEntity>, IYourEntityRepository
{
    public YourEntityRepository(SocietyContext context) : base(context) { }

    public async Task<YourEntity?> GetByNameAsync(string name)
    {
        return await _context.Set<YourEntity>()
            .FirstOrDefaultAsync(e => e.Name == name);
    }
}
```

### E. Create Application Service

Create in `SocietySaaS.Application/Services/`:

```csharp
namespace SocietySaaS.Application.Services;

public interface IYourEntityService
{
    Task<YourEntityDto> GetByIdAsync(int id);
    Task<IReadOnlyList<YourEntityDto>> GetAllAsync();
    Task<YourEntityDto> CreateAsync(CreateYourEntityCommand command);
}

public class YourEntityService : IYourEntityService
{
    private readonly IYourEntityRepository _repository;
    private readonly IMapper _mapper;

    public YourEntityService(IYourEntityRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<YourEntityDto> CreateAsync(CreateYourEntityCommand command)
    {
        var entity = _mapper.Map<YourEntity>(command);
        await _repository.AddAsync(entity);
        return _mapper.Map<YourEntityDto>(entity);
    }
}
```

### F. Create Controller

Create in `SocietySaaS.API/Controllers/`:

```csharp
namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class YourEntitiesController : ControllerBase
{
    private readonly IYourEntityService _service;

    public YourEntitiesController(IYourEntityService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<YourEntityDto>>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(new ApiResponse<List<YourEntityDto>>
        {
            Success = true,
            Data = result.ToList(),
            Timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<YourEntityDto>>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(new ApiResponse<YourEntityDto>
        {
            Success = true,
            Data = result,
            Timestamp = DateTime.UtcNow
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<YourEntityDto>>> Create(
        CreateYourEntityCommand command)
    {
        var result = await _service.CreateAsync(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            new ApiResponse<YourEntityDto>
            {
                Success = true,
                Data = result,
                Message = "Created successfully",
                Timestamp = DateTime.UtcNow
            });
    }
}
```

---

## 3. Implementation Requirements

### Code Standards (MUST Follow)

**Read first**: See `docs/architecture/coding-standards.md`

- **Classes/Interfaces**: PascalCase (`SocietyService`, `ISocietyRepository`)
- **Variables/Parameters**: camelCase (`societyName`, `pageSize`)
- **Constants**: UPPER_SNAKE_CASE (`MAX_RETRY_ATTEMPTS`)
- **Private fields**: _camelCase (`_logger`, `_repository`)

### Architecture (MUST Follow)

**Read first**: See `docs/architecture/architecture.md`

Each module follows Clean Architecture layers:

| Layer | Responsibility | Example |
|-------|-----------------|---------|
| **Domain** | Core business logic, entities | `Society`, `Bill` entities |
| **Application** | Use cases, DTOs, validation | `CreateSocietyUseCase`, `SocietyDto` |
| **Infrastructure** | Database, APIs, external services | `SocietyRepository`, `EmailService` |
| **API** | HTTP endpoints, controllers | `SocietiesController` |

**No layer mixing**: Infrastructure cannot call Application!

### API Design (MUST Follow)

**Read first**: See `docs/api/api-guidelines.md`

Template endpoints:
```
GET    /api/v1/[resources]              # List all
GET    /api/v1/[resources]/{id}         # Get one
POST   /api/v1/[resources]              # Create
PUT    /api/v1/[resources]/{id}         # Update
DELETE /api/v1/[resources]/{id}         # Delete
```

Required response format:
```json
{
  "success": true,
  "data": { /* your data */ },
  "message": "Operation completed",
  "timestamp": "2026-09-15T10:30:00Z"
}
```

### Database Setup (MUST Do)

1. Create Entity Framework DbContext in `Infrastructure/Data`
2. Add migrations:
   ```bash
   dotnet ef migrations add AddYourEntity --project SocietySaaS.Infrastructure --startup-project SocietySaaS.API
   dotnet ef database update --project SocietySaaS.Infrastructure --startup-project SocietySaaS.API
   ```
3. Use repositories (don't access DbContext directly from controllers)
4. Add connection string to `appsettings.json`

### Testing (MUST Have)

**Minimum coverage: 80%**

- **Unit Tests**: Test business logic in isolation
- **Integration Tests**: Test database + API together
- **E2E Tests**: Test complete workflows

```bash
# Run all tests
dotnet test

# With coverage
dotnet test /p:CollectCoverage=true
```

---

## 4. CI/CD Integration

The module will automatically be included in CI/CD since it's part of the monolith.

### Build Pipeline Steps

```bash
# Restore dependencies
dotnet restore

# Run tests
dotnet test

# Build
dotnet build --configuration Release
```

---

## 5. Quality Gates (MUST Pass)

Before merging to `main`, verify:
- [ ] Code follows coding standards
- [ ] All tests pass -> `dotnet test`
- [ ] Test coverage >= 80% -> Coverage report
- [ ] No security vulnerabilities
- [ ] API documentation complete -> Swagger works
- [ ] Code review approved by 1+ team members

---

## 6. Checklist: Module Ready for Production?

```
Development
- [ ] Code written & tested (80%+ coverage)
- [ ] Code review approved
- [ ] API documentation complete
- [ ] Database migrations working
- [ ] Local testing successful

Deployment
- [ ] Environment variables configured
- [ ] Logging configured (Application Insights)
- [ ] Health check endpoint added

Operations
- [ ] Monitoring alerts configured
- [ ] Rollback strategy documented
```

---

## Reference Documents

Read in this order:
1. `docs/architecture/architecture.md` - System design
2. `docs/architecture/coding-standards.md` - Code style
3. `docs/api/api-guidelines.md` - API design
4. `docs/architecture/ci-cd.md` - Deployment pipeline
5. `docs/architecture/testing.md` - Test strategies
6. `docs/architecture/code-review.md` - PR process
7. `docs/architecture/troubleshooting.md` - Fix issues

---

## Common Questions

### Q: How do I add a database table?
A: Create a new entity in `Domain/Entities/`, then run:
```bash
dotnet ef migrations add AddYourTable --project SocietySaaS.Infrastructure --startup-project SocietySaaS.API
dotnet ef database update --project SocietySaaS.Infrastructure --startup-project SocietySaaS.API
```

### Q: How do I add a new API endpoint?
A: Create a new controller or add to existing one in `API/Controllers/`. Follow the pattern in section 2F.

### Q: How do I add authentication?
A: See existing controllers for JWT or OTP setup. Use `[Authorize]` attribute on protected endpoints.

### Q: How do I add validation?
A: Create a FluentValidation validator in `Application/Validators/`:
```csharp
public class CreateYourEntityValidator : AbstractValidator<CreateYourEntityCommand> {
    public CreateYourEntityValidator() {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
```
