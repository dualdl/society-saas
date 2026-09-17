# Troubleshooting Guide

## Table of Contents
1. [Common Backend Issues](#common-backend-issues)
2. [Common Frontend Issues](#common-frontend-issues)
3. [Database Issues](#database-issues)
4. [Azure Issues](#azure-issues)
5. [CI/CD Issues](#cicd-issues)
6. [Performance Issues](#performance-issues)

---

## Common Backend Issues

### Service Won't Start

**Symptoms**: Application crash on startup, process exits immediately

```bash
# Step 1: Check logs
dotnet run > logs.txt 2>&1

# Step 2: Verify .NET version
dotnet --version  # Should be 10.0+

# Step 3: Check dependencies
dotnet restore
```

**Common Causes**:
1. Missing environment variables
2. Database connection string invalid
3. NuGet package conflicts
4. Port already in use

**Solution**:
```csharp
// Add comprehensive logging
var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Check required config
var dbConnection = builder.Configuration["ConnectionStrings:DefaultConnection"]
    ?? throw new InvalidOperationException("Missing DefaultConnection");
```

### API Endpoint Returns 500 Error

**Symptoms**: `HTTP 500 Internal Server Error`

```bash
# Step 1: Check application logs
az webapp log tail --name society-saas-api --resource-group society-saas-rg

# Step 2: Check specific service logs in Application Insights
```

**Common Causes**:
1. Null reference exception
2. Database query error
3. Third-party service call failed
4. Missing configuration

**Solution**:
```csharp
[HttpPost]
public async Task<ActionResult<ApiResponse<SocietyDto>>> CreateAsync(
    CreateSocietyCommand command) {
    try {
        _logger.LogInformation("Creating society {Name}", command.Name);

        var society = await _service.CreateAsync(command);
        return CreatedAtAction(nameof(GetAsync),
            new { id = society.Id },
            new ApiResponse<SocietyDto> {
                Success = true,
                Data = society,
                Message = "Society created successfully"
            });
    } catch (ValidationException ex) {
        return BadRequest(new ApiResponse<object> {
            Success = false,
            Message = ex.Message
        });
    } catch (Exception ex) {
        _logger.LogError(ex, "Error creating society");
        return StatusCode(500, new ApiResponse<object> {
            Success = false,
            Message = "An unexpected error occurred"
        });
    }
}
```

### Memory Leak

**Symptoms**: Process memory usage increases over time, app crashes with out-of-memory

```bash
# Check memory usage
docker stats <container-id>

# Monitor in Azure
# Azure Portal -> App Service -> Metrics -> Memory Working Set
```

**Common Causes**:
1. Event subscriptions not unsubscribed
2. Unbounded collections growing
3. Database connection not disposed

**Solution**:
```csharp
// Implement IDisposable
public class SocietyService : IDisposable {
    private IDisposable? _subscription;

    public void Subscribe(IEventBus eventBus) {
        _subscription = eventBus.Subscribe<SocietyCreatedEvent>(OnSocietyCreated);
    }

    public void Dispose() {
        _subscription?.Dispose();
    }
}
```

---

## Common Frontend Issues

### Blank/White Screen

**Symptoms**: Browser shows nothing, no console errors

**Debug Steps**:
```typescript
// 1. Check browser console
console.log('App starting...');

// 2. Check network requests
// DevTools -> Network tab -> Check for failed requests

// 3. Verify React mounting
if (!document.getElementById('root')) {
    throw new Error('Root element not found');
}
```

### API Calls Fail with CORS Error

**Symptoms**: Console error: `Access to XMLHttpRequest blocked by CORS policy`

**Solution - Backend**:
```csharp
// In Program.cs
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(builder => {
        builder
            .WithOrigins("http://localhost:3000", "https://society-saas-web.azurewebsites.net")
            .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH")
            .WithHeaders("Content-Type", "Authorization")
            .AllowCredentials();
    });
});

app.UseCors();
```

**Solution - Frontend**:
```typescript
// Ensure credentials are sent
const response = await fetch('/api/endpoint', {
    method: 'GET',
    credentials: 'include',
    headers: {
        'Content-Type': 'application/json'
    }
});
```

### State Not Updating / Infinite Render Loop

**Symptoms**: Component doesn't respond to data changes, or renders forever

**Solution**:
```typescript
// WRONG: Missing dependency array
useEffect(() => {
    setData(fetchData());
    // Runs on EVERY render!
});

// CORRECT: Dependency array
useEffect(() => {
    const fetchData = async () => {
       const res = await fetch('/api/data');
       setData(await res.json());
    };
    fetchData();
}, []);  // Runs only on mount
```

---

## Database Issues

### Migrations Won't Apply

**Symptoms**: `Pending migrations` error, database schema mismatch

```bash
# Check pending migrations
dotnet ef migrations list

# Apply pending migrations
dotnet ef database update
```

**Solution**:
```csharp
// In Program.cs: Auto-apply migrations on startup
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<SocietyContext>();
    db.Database.Migrate();
}
```

### Primary Key/Unique Constraint Conflict

**Symptoms**: `Violation of PRIMARY KEY constraint`, `UNIQUE constraint failed`

```bash
# Check duplicate data
SELECT * FROM Societies WHERE Name = 'Green Valley'

# Find conflicting records
SELECT Name, COUNT(*)
FROM Societies
GROUP BY Name
HAVING COUNT(*) > 1
```

**Solution**:
```sql
-- Remove duplicates (keep first)
WITH CTE AS (
  SELECT *, ROW_NUMBER() OVER (PARTITION BY Name ORDER BY Id) as rn
  FROM Societies
)
DELETE FROM CTE WHERE rn > 1
```

### Connection Timeout

**Symptoms**: `Timeout expired`, `Connection broken`

```csharp
// Increase timeout
var options = new DbContextOptionsBuilder<SocietyContext>()
    .UseSqlServer(connectionString, options => {
        options.CommandTimeout(60);
        options.EnableRetryOnFailure(maxRetryCount: 3);
    })
    .Options;
```

---

## Azure Issues

### Key Vault Access Denied

**Symptoms**: `Access Denied` when reading from Azure Key Vault

```bash
# Check managed identity has access
az identity show --ids <identity-id>

# Verify RBAC role assigned
az role assignment list --assignee <object-id>
```

### Application Insights Not Showing Telemetry

**Symptoms**: No requests, errors, or metrics visible in App Insights

```csharp
// Verify instrumentation key configured
var instrumentationKey = builder.Configuration["APPINSIGHTS_INSTRUMENTATION_KEY"];
if (string.IsNullOrEmpty(instrumentationKey)) {
    throw new InvalidOperationException("Missing APPINSIGHTS_INSTRUMENTATION_KEY");
}

builder.Services.AddApplicationInsightsTelemetry(instrumentationKey);
```

---

## CI/CD Issues

### Build Fails with NuGet Error

**Symptoms**: `Unable to load dependent dll`, `NuGet restore failed`

```bash
# Solution: Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore

# Build with verbose output
dotnet build --verbosity diagnostic
```

### Test Fails Intermittently (Flaky Test)

**Symptoms**: Test passes sometimes, fails other times

**Solution**:
```csharp
// Don't use DateTime directly
[Test]
public void Test_WithStaticTime() {
    var time = DateTime.Now;  // Different each run
}

// Use injectable time provider
[Test]
public void Test_WithMockTime() {
    var mockTime = new Mock<IDateTimeProvider>();
    mockTime.Setup(x => x.UtcNow).Returns(new DateTime(2026, 9, 15));
    var result = _service.Calculate(mockTime.Object);
}
```

---

## Performance Issues

### Slow API Response (> 2 seconds)

**Steps**:
```bash
# 1. Check API response time
time curl https://society-saas-api.azurewebsites.net/api/v1/societies

# 2. Check Application Insights
# Dependency tracking -> Find slow dependencies
```

**Solution**: Implement caching
```csharp
public async Task<List<SocietyDto>> GetSocietiesAsync() {
    var cacheKey = "societies_all";

    if (!_cache.TryGetValue(cacheKey, out List<SocietyDto> societies)) {
        societies = await _repo.GetAllAsync();
        _cache.Set(cacheKey, societies, TimeSpan.FromHours(1));
    }

    return societies;
}
```

### High Memory Usage

**Symptoms**: Process uses 1GB+, risk of OOM killer

**Solution**: Implement object pooling
```csharp
// Use ArrayPool for temporary buffers
public async Task ProcessLargeFileAsync(Stream stream) {
    byte[] buffer = ArrayPool<byte>.Shared.Rent(4096);
    try {
        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0) {
            ProcessBuffer(buffer, bytesRead);
        }
    } finally {
        ArrayPool<byte>.Shared.Return(buffer);
    }
}
```

---

## Getting Help

### Debugging Checklist

- [ ] Check application logs (on-disk or cloud)
- [ ] Search Application Insights
- [ ] Review recent code changes
- [ ] Check external service status
- [ ] Verify environment variables
- [ ] Check database connectivity
- [ ] Review resource limits (CPU, memory)
- [ ] Check network/firewall rules

### Escalation Path

1. **Junior Dev**: Check logs and known issues
2. **Senior Dev**: Deep debugging, code review
3. **Tech Lead**: Architecture decisions
4. **On-Call SRE**: Production incidents

### Resources

- Architecture docs: See `docs/architecture/architecture.md`
- Coding standards: See `docs/architecture/coding-standards.md`
- Azure docs: https://docs.microsoft.com/azure
- .NET docs: https://docs.microsoft.com/dotnet
