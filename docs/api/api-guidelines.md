# API Guidelines

## Table of Contents
1. [REST Principles](#rest-principles)
2. [API Versioning](#api-versioning)
3. [Request/Response Format](#requestresponse-format)
4. [Status Codes](#status-codes)
5. [Validation](#validation)
6. [Authentication & Authorization](#authentication--authorization)
7. [Error Handling](#error-handling)
8. [Pagination & Filtering](#pagination--filtering)

---

## REST Principles

### HTTP Methods

```
GET     -> Retrieve resource (safe, idempotent)
POST    -> Create new resource
PUT     -> Update entire resource (idempotent)
PATCH   -> Partial update
DELETE  -> Remove resource (idempotent)
```

### Resource-Oriented URLs

```
CORRECT: Resource-focused
GET    /api/v1/societies              (get all)
GET    /api/v1/societies/{id}         (get one)
POST   /api/v1/societies              (create)
PUT    /api/v1/societies/{id}         (replace)
PATCH  /api/v1/societies/{id}         (update)
DELETE /api/v1/societies/{id}         (delete)

GET    /api/v1/societies/{id}/flats   (nested resource)
POST   /api/v1/societies/{id}/flats   (create nested)

WRONG: Action-focused (RPC-style)
GET    /api/GetSociety
POST   /api/CreateSociety
POST   /api/UpdateSociety
POST   /api/DeleteSociety
```

### No Verbs in URLs

```
WRONG
POST /api/v1/societies/calculateBill
GET  /api/v1/reports/generateBillReport

CORRECT
POST /api/v1/billing/calculations          (noun-based)
GET  /api/v1/billing/reports               (noun-based)
```

---

## API Versioning

### URL-Based Versioning (Recommended)

```
GET /api/v1/societies
GET /api/v2/societies    (breaking change)
```

### Implementation in .NET

```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class SocietiesController : ControllerBase {

    // Available in v1 and v2
    [HttpGet("{id}")]
    public async Task<ActionResult<SocietyDto>> GetAsync(int id) { }

    // Only in v2 (breaking change)
    [HttpGet("{id}")]
    [MapToApiVersion("2.0")]
    public async Task<ActionResult<SocietyDtoV2>> GetV2Async(int id) { }
}
```

### Deprecation Strategy

```
API v1 (Current)
 - Supported: 12 months
 - Timeline: Announced 3 months before EOL
 - Migration guide: Published 6 months before

API v2 (New)
 - Breaking changes documented
 - Migration examples provided
 - v1 continues working (minimum 12 months)
```

---

## Request/Response Format

### Standard Response Envelope

```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Green Valley",
    "area": "1000"
  },
  "message": "Operation completed",
  "timestamp": "2026-09-15T10:30:00Z"
}
```

### Error Response Format

```json
{
  "success": false,
  "data": null,
  "message": "Validation failed",
  "errors": [
    {
      "code": "VALIDATION_ERROR",
      "message": "Name is required",
      "field": "name"
    }
  ],
  "timestamp": "2026-09-15T10:30:00Z"
}
```

### Implementation

```csharp
public class ApiResponse<T> {
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public DateTime Timestamp { get; set; }
    public List<ApiError>? Errors { get; set; }
}

public class ApiError {
    public string Code { get; set; }
    public string Message { get; set; }
    public string? Field { get; set; }
}

// Usage
return Ok(new ApiResponse<SocietyDto> {
    Success = true,
    Data = society,
    Message = "Society created successfully",
    Timestamp = DateTime.UtcNow
});
```

---

## Status Codes

### Success Responses (2xx)

```
200 OK              -> Request succeeded, returning data
201 Created         -> Resource created successfully
202 Accepted        -> Request accepted, processing async
204 No Content      -> Request succeeded, no data to return
```

### Client Errors (4xx)

```
400 Bad Request     -> Invalid request format/data
401 Unauthorized    -> Missing/invalid authentication
403 Forbidden       -> Authenticated but no permission
404 Not Found       -> Resource doesn't exist
409 Conflict        -> Request conflicts with state (e.g., duplicate)
422 Unprocessable   -> Validation failed
429 Too Many Req    -> Rate limit exceeded
```

### Server Errors (5xx)

```
500 Internal Error  -> Unexpected error
502 Bad Gateway     -> Upstream service unavailable
503 Service Down    -> Service temporarily unavailable
504 Gateway Timeout -> Upstream timeout
```

---

## Validation

### Request Validation

```csharp
// CORRECT: Using FluentValidation
public class CreateSocietyCommandValidator : AbstractValidator<CreateSocietyCommand> {

    public CreateSocietyCommandValidator() {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must be <= 100 characters");

        RuleFor(x => x.Area)
            .NotEmpty().WithMessage("Area is required");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required");
    }
}
```

### Input Sanitization

```csharp
// CORRECT: Sanitize strings
public string SanitizeInput(string input) {
    return System.Web.HttpUtility.HtmlEncode(input)?.Trim() ?? string.Empty;
}

// Prevent SQL Injection (always use parameterized queries)
var society = await _context.Societies
    .FirstOrDefaultAsync(s => s.Id == id);  // Safe
```

---

## Authentication & Authorization

### JWT Authentication

```csharp
// In Program.cs
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new() {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization(options => {
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("SocietyAccess", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("permissions", "society:read")));
});

// In controller
[Authorize]
[HttpGet("{id}")]
public async Task<ActionResult<SocietyDto>> GetAsync(int id) { }

[Authorize(Roles = "Admin")]
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteAsync(int id) { }
```

---

## Error Handling

### Centralized Exception Handling

```csharp
// Middleware
app.UseExceptionHandler(errorApp => {
    errorApp.Run(async context => {
        var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
        var response = MapExceptionToResponse(exception);

        context.Response.StatusCode = response.StatusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(response);
    });
});

private static (int StatusCode, object Response) MapExceptionToResponse(Exception? exception) {
    return exception switch {
        NotFoundException ex => (404, new { success = false, message = ex.Message }),
        ValidationException ex => (422, new { success = false, message = ex.Message }),
        UnauthorizedAccessException => (403, new { success = false, message = "Access denied" }),
        _ => (500, new { success = false, message = "An unexpected error occurred" })
    };
}
```

---

## Pagination & Filtering

### Query Parameters

```
GET /api/v1/societies?page=1&pageSize=20&sort=name&filter=city:Mumbai
```

### Response with Pagination

```json
{
  "success": true,
  "data": [ /* societies */ ],
  "metadata": {
    "page": 1,
    "pageSize": 20,
    "total": 150,
    "totalPages": 8
  }
}
```

---

## Documentation

### OpenAPI/Swagger

```csharp
// In Program.cs
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new OpenApiInfo {
        Title = "Society SaaS API",
        Version = "v1.0.0",
        Description = "Society management platform API"
    });
});

// Access Swagger UI
// https://your-api.com/swagger
```
