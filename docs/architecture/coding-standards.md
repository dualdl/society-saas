# Coding Standards

## Table of Contents
1. [Backend (.NET 10)](#backend-net-10)
2. [Frontend (React 18+)](#frontend-react-18)
3. [General Rules](#general-rules)
4. [Enforcement](#enforcement)

---

## Backend (.NET 10)

### Naming Conventions

```csharp
// Classes, Interfaces, Methods → PascalCase
public class SocietyService { }
public interface ISocietyRepository { }
public async Task<SocietyDto> GetSocietyAsync(int id) { }

// Variables, Parameters, Properties → camelCase
public string societyName;
private readonly ILogger _logger;
public int pageSize { get; set; }

// Constants → UPPER_SNAKE_CASE
public const string DATABASE_CONNECTION_KEY = "DefaultConnection";
public const int MAX_RETRY_ATTEMPTS = 3;

// Enum values → PascalCase
public enum SocietyStatus { Active, Inactive, Suspended }

// Private fields → _camelCase
private readonly IUnitOfWork _unitOfWork;
private int _retryCount;
```

### SOLID Principles (MANDATORY)

#### S - Single Responsibility Principle
```csharp
// WRONG: Multiple responsibilities
public class SocietyService {
    public void CreateSociety() { /* ... */ }
    public void SendEmail() { /* ... */ }
    public void LogToDatabase() { /* ... */ }
}

// CORRECT: Separated concerns
public class SocietyService {
    public SocietyDto Create(CreateSocietyCommand cmd) { /* ... */ }
}

public class SocietyNotificationService {
    public Task NotifyAsync(SocietyDto society) { /* ... */ }
}
```

#### O - Open/Closed Principle
```csharp
// WRONG: Hard to extend
public class BillCalculator {
    public decimal Calculate(Society society, string type) {
        if (type == "maintenance") return society.Area * 10;
        if (type == "water") return society.Area * 5;
        throw new ArgumentException();
    }
}

// CORRECT: Open for extension
public interface IBillCalculationStrategy {
    decimal Calculate(Society society);
}

public class MaintenanceBillCalculator : IBillCalculationStrategy {
    public decimal Calculate(Society society) => society.Area * 10m;
}
```

#### D - Dependency Inversion Principle
```csharp
// WRONG: High-level depends on low-level
public class SocietyService {
    private SqlServerDatabase _db = new();
    public void Process() => _db.Save();
}

// CORRECT: Both depend on abstraction
public interface ISocietyRepository {
    Task SaveAsync(Society society);
}

public class SocietyService {
    private readonly ISocietyRepository _repo;
    public SocietyService(ISocietyRepository repo) => _repo = repo;
}
```

### Method Size & Complexity

```csharp
// GOOD: Small, focused methods
public async Task<SocietyDto> GetSocietyAsync(int id) {
    var society = await _repository.GetByIdAsync(id);
    return _mapper.Map<SocietyDto>(society);
}

// WRONG: God method > 50 lines
public async Task<SocietyDto> GetSocietyAsync(int id) {
    // 100 lines of nested logic...
}
```

**Rule**: Keep methods < 50 lines. If longer, extract into smaller methods.

### Error Handling

```csharp
// CORRECT: Global exception middleware
app.UseExceptionHandler(errorApp => {
    errorApp.Run(async context => {
        var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
        var response = new { error = exception?.Message };
        await context.Response.WriteAsJsonAsync(response);
    });
});

// In services: Let exceptions bubble up
public async Task<Society> GetSocietyAsync(int id) {
    var society = await _context.Societies.FindAsync(id);
    if (society == null)
        throw new NotFoundException($"Society {id} not found");
    return society;
}
```

### Dependency Injection

```csharp
// CORRECT: Always use DI
public class SocietyService {
    private readonly ISocietyRepository _repo;
    private readonly ILogger<SocietyService> _logger;

    public SocietyService(ISocietyRepository repo, ILogger<SocietyService> logger) {
        _repo = repo;
        _logger = logger;
    }
}

// WRONG: Static classes or `new` keyword
var db = new SocietyContext();
```

### API Endpoints

```csharp
// CORRECT: RESTful with versioning
[ApiController]
[Route("api/v1/[controller]")]
public class SocietiesController : ControllerBase {
    [HttpGet("{id}")]
    public async Task<ActionResult<SocietyDto>> GetAsync(int id) { }

    [HttpPost]
    public async Task<ActionResult<SocietyDto>> CreateAsync(CreateSocietyCommand cmd) { }

    [HttpPut("{id}")]
    public async Task<ActionResult<SocietyDto>> UpdateAsync(int id, UpdateSocietyCommand cmd) { }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id) { }
}

// Standard Response Format
public class ApiResponse<T> {
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
}
```

### Async/Await

```csharp
// CORRECT: Always async with Task
public async Task<Society> GetSocietyAsync(int id) {
    return await _repository.Societies
        .FirstOrDefaultAsync(s => s.Id == id);
}

// WRONG: Sync over async, .Result blocking
public Society GetSociety(int id) {
    return _repository.GetSocietyAsync(id).Result; // DEADLOCK RISK!
}
```

---

## Frontend (React 18+)

### Component Structure

```typescript
// CORRECT: Functional component with TypeScript
import React from 'react';
import { useQuery } from 'react-query';
import { SocietyApiClient } from '../api';
import { SocietyCard } from './SocietyCard';

interface SocietyListProps {
  onSelect: (id: number) => void;
}

export const SocietyList: React.FC<SocietyListProps> = ({ onSelect }) => {
  const { data: societies, isLoading, error } = useQuery(
    ['societies'],
    () => SocietyApiClient.getAll()
  );

  if (isLoading) return <CircularProgress />;
  if (error) return <Alert severity="error">Error loading societies</Alert>;

  return (
    <Box>
      {societies?.map((society) => (
        <SocietyCard
          key={society.id}
          society={society}
          onSelect={() => onSelect(society.id)}
        />
      ))}
    </Box>
  );
};
```

### Hooks Best Practices

```typescript
// CORRECT: Custom hook for logic reuse
export const useSocieties = () => {
  const [filter, setFilter] = useState('');

  const { data, isLoading } = useQuery(
    ['societies', filter],
    () => SocietyApiClient.search(filter)
  );

  return { societies: data, isLoading, setFilter };
};

// Usage in component
const { societies, isLoading, setFilter } = useSocieties();
```

### Naming Conventions

```typescript
// React components → PascalCase
export const SocietyCard = () => { };
export const BillingForm = () => { };

// Custom hooks → useXxx (camelCase with 'use' prefix)
export const useSocieties = () => { };
export const useFetchBills = () => { };

// Variables, functions → camelCase
const societyCount = 10;
const getSocietyById = (id: number) => { };
const handleSubmit = (e: React.FormEvent) => { };

// Constants → UPPER_SNAKE_CASE
const MAX_SOCIETIES_PER_PAGE = 50;
const API_TIMEOUT_MS = 5000;

// Types/Interfaces → PascalCase
interface Society {
  id: number;
  name: string;
}

type BillingStatus = 'pending' | 'paid' | 'overdue';
```

### TypeScript Strictness

```typescript
// CORRECT: Strict types, no 'any'
interface SocietyRequest {
  name: string;
  area: string;
  city: string;
}

const createSociety = (req: SocietyRequest): Promise<SocietyDto> => {
  return apiClient.post('/societies', req);
};

// WRONG: 'any' type
const createSociety = (req: any): any => {
  return apiClient.post('/societies', req);
};
```

---

## General Rules (All Languages)

### Environment Configuration

```typescript
// CORRECT: Use environment variables
const apiUrl = process.env.REACT_APP_API_URL;

// CORRECT: .env.development, .env.production
// .env.development
REACT_APP_API_URL=http://localhost:5000
DEBUG=true

// .env.production
REACT_APP_API_URL=https://society-saas-api.azurewebsites.net
DEBUG=false

// WRONG: Hardcoded values
const apiUrl = "http://localhost:5000";
```

### Secrets & Security

```csharp
// CORRECT: Use Azure Key Vault
var builder = new ConfigurationBuilder()
    .AddAzureKeyVault(new Uri("https://vault.azure.net/"), new DefaultAzureCredential());

// WRONG: Hardcoded secrets
private const string API_KEY = "sk-1234567890abcdef";
```

### Logging

```csharp
// CORRECT: Structured logging
_logger.LogInformation("Processing society {SocietyId}", societyId);
_logger.LogError(ex, "Failed to create bill for society {SocietyId}", societyId);

// WRONG: String concatenation
_logger.LogInformation("Processing society " + societyId);
```

---

## Enforcement

### Pre-commit Hooks

```bash
# Backend: StyleCop
<PropertyGroup>
  <EnableNETAnalyzers>true</EnableNETAnalyzers>
  <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
</PropertyGroup>

# Frontend: ESLint + Prettier
npm install -D eslint prettier @typescript-eslint/eslint-plugin
```

### Code Review Checklist

- [ ] Follows naming conventions
- [ ] No violations of SOLID principles
- [ ] Methods/functions are < 50 lines
- [ ] No hardcoded values
- [ ] Proper error handling
- [ ] Meaningful variable names

---

## Tools Recommended

| Tool | Purpose |
|------|---------|
| **SonarQube** | Code quality analysis |
| **ReSharper** | .NET code inspector |
| **ESLint** | JavaScript/TypeScript linting |
| **Prettier** | Code formatter |
| **StyleCop** | C# style enforcement |
