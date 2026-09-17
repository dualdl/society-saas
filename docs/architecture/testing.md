# Testing Strategy

## Table of Contents
1. [Testing Pyramid](#testing-pyramid)
2. [Unit Tests](#unit-tests)
3. [Integration Tests](#integration-tests)
4. [Component Tests (Frontend)](#component-tests-frontend)
5. [Test Coverage](#test-coverage)
6. [Best Practices](#best-practices)

---

## Testing Pyramid

```
       /\ 
      /  \  E2E & Manual Tests (Few)
     /----\
    /      \
   /--------\ Integration Tests (Some)
  /          \
 /------------\ Unit Tests (Many)
/______________\

Rule: More unit tests, fewer E2E tests
Cost: Unit tests cheap, E2E tests expensive
```

### Distribution

- **70%** Unit Tests (Fast, Isolated, Cheap)
- **20%** Integration Tests (Medium speed, DB involved)
- **10%** E2E Tests (Slow, Full system, Expensive)

---

## Unit Tests

### Backend: xUnit / NUnit

```csharp
// CORRECT: Unit test structure
[TestFixture]
public class SocietyServiceTests {

    private SocietyService _service;
    private Mock<ISocietyRepository> _repoMock;

    [SetUp]
    public void Setup() {
        _repoMock = new Mock<ISocietyRepository>();
        _service = new SocietyService(_repoMock.Object);
    }

    // Test naming: MethodName_Should_ExpectedBehavior
    [Test]
    public async Task GetSociety_WithValidId_ShouldReturnSociety() {
        // Arrange
        var society = new Society { Id = 1, Name = "Test Society" };
        _repoMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(society);

        // Act
        var result = await _service.GetSocietyAsync(1);

        // Assert
        Assert.AreEqual("Test Society", result.Name);
        _repoMock.Verify(x => x.GetByIdAsync(1), Times.Once);
    }

    [Test]
    public async Task GetSociety_WithInvalidId_ShouldThrowNotFoundException() {
        // Arrange
        _repoMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Society)null);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(
            async () => await _service.GetSocietyAsync(999)
        );
    }

    [Test]
    [TestCase("Society A", 100, 1000)]
    [TestCase("Society B", 200, 2000)]
    public async Task CreateSociety_WithVariousInputs_ShouldReturnCorrectAmount(
        string name, decimal area, decimal expected) {
        // Arrange
        var command = new CreateSocietyCommand { Name = name, Area = area };

        // Act
        var result = await _service.CreateSocietyAsync(command);

        // Assert
        Assert.AreEqual(expected, result.Area);
    }
}
```

### Frontend: Jest + React Testing Library

```typescript
// CORRECT: React component test
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { SocietyForm } from './SocietyForm';

describe('SocietyForm', () => {

    test('should display form fields on render', () => {
        render(<SocietyForm onSubmit={jest.fn()} />);

        expect(screen.getByLabelText(/society name/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/area/i)).toBeInTheDocument();
        expect(screen.getByRole('button', { name: /submit/i })).toBeInTheDocument();
    });

    test('should call onSubmit with valid data', async () => {
        const mockSubmit = jest.fn();
        const user = userEvent.setup();

        render(<SocietyForm onSubmit={mockSubmit} />);

        await user.type(screen.getByLabelText(/society name/i), 'Green Valley');
        await user.type(screen.getByLabelText(/area/i), '1000');
        await user.click(screen.getByRole('button', { name: /submit/i }));

        await waitFor(() => {
            expect(mockSubmit).toHaveBeenCalledWith({
                name: 'Green Valley',
                area: '1000'
            });
        });
    });

    test('should display validation error for empty name', async () => {
        const user = userEvent.setup();
        render(<SocietyForm onSubmit={jest.fn()} />);

        await user.click(screen.getByRole('button', { name: /submit/i }));

        expect(screen.getByText(/name is required/i)).toBeInTheDocument();
    });
});
```

---

## Integration Tests

### Database Integration

```csharp
// CORRECT: Integration test with test database
[TestFixture]
public class SocietyRepositoryIntegrationTests : IAsyncLifetime {

    private SqliteConnection _connection;
    private SocietyContext _dbContext;
    private SocietyRepository _repository;

    public async Task InitializeAsync() {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        var options = new DbContextOptionsBuilder<SocietyContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new SocietyContext(options);
        await _dbContext.Database.EnsureCreatedAsync();
        _repository = new SocietyRepository(_dbContext);
    }

    public async Task DisposeAsync() {
        await _dbContext.DisposeAsync();
        _connection.Close();
    }

    [Test]
    public async Task AddSociety_WithValidData_ShouldPersistToDatabase() {
        // Arrange
        var society = new Society {
            Name = "Green Valley",
            Area = "1000",
            City = "Mumbai"
        };

        // Act
        await _repository.AddAsync(society);
        await _dbContext.SaveChangesAsync();

        // Assert
        var saved = await _dbContext.Societies.FirstOrDefaultAsync(s => s.Id == society.Id);
        Assert.IsNotNull(saved);
        Assert.AreEqual("Green Valley", saved.Name);
    }
}
```

### API Integration Test

```csharp
// CORRECT: API integration test
[TestFixture]
public class SocietyApiIntegrationTests : IAsyncLifetime {

    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    public async Task InitializeAsync() {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync() {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GetSocieties_WhenCalled_ShouldReturnOk() {
        // Act
        var response = await _client.GetAsync("/api/v1/admin/societies");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [Test]
    public async Task CreateSociety_WithValidData_ShouldReturnCreated() {
        // Arrange
        var command = new { name = "Green Valley", area = "1000", city = "Mumbai" };
        var json = JsonConvert.SerializeObject(command);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/admin/societies", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
    }
}
```

---

## Component Tests (Frontend)

```typescript
// CORRECT: Component integration test
import { render, screen, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from 'react-query';
import { SocietyList } from './SocietyList';

describe('SocietyList Integration', () => {

    test('should fetch and display societies on load', async () => {
        const queryClient = new QueryClient({
            defaultOptions: {
                queries: { retry: false }
            }
        });

        render(
            <QueryClientProvider client={queryClient}>
                <SocietyList onSelect={jest.fn()} />
            </QueryClientProvider>
        );

        // Should show loading state initially
        expect(screen.getByRole('progressbar')).toBeInTheDocument();

        // Should display societies after fetch
        await waitFor(() => {
            expect(screen.getByText('Green Valley')).toBeInTheDocument();
            expect(screen.getByText('Sunrise Society')).toBeInTheDocument();
        });
    });

    test('should display error message on fetch failure', async () => {
        jest.spyOn(global, 'fetch').mockRejectedValueOnce(new Error('API Error'));

        render(<SocietyList onSelect={jest.fn()} />);

        await waitFor(() => {
            expect(screen.getByText(/error/i)).toBeInTheDocument();
        });
    });
});
```

---

## Test Coverage

### Coverage Goals

| Component | Target | Rationale |
|-----------|--------|-----------|
| **Business Logic** | 90%+ | Critical, highest risk |
| **API Controllers** | 80%+ | Important for integration |
| **Utility Functions** | 85%+ | Reused widely |
| **UI Components** | 70%+ | Lower risk if not tested |
| **Overall** | 80%+ | Team standard |

### Measuring Coverage

```bash
# Backend: Code coverage
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura

# Frontend: Jest coverage
npm run test:coverage

# Generate HTML report
npx nyc report --reporter=html
```

### What NOT to Test

**Don't test**:
- Framework code (EF Core, ASP.NET Core)
- Third-party libraries
- Generated code
- Simple getters/setters

**Do test**:
- Business logic
- Algorithms
- Validation rules
- Error handling
- Edge cases

---

## Test Organization

### Folder Structure

```
tests/
├── SocietySaaS.UnitTests/
│   ├── Domain/
│   ├── Application/
│   └── Infrastructure/
│
├── SocietySaaS.IntegrationTests/
│   ├── Api/
│   ├── Repository/
│   └── Services/
│
└── SocietySaaS.E2ETests/
    ├── Scenarios/
    └── Fixtures/
```

### Naming Conventions

```csharp
// Convention: [MethodName]_[Scenario]_[Expected]

// CORRECT:
GetSociety_WithValidId_ReturnsSociety
GetSociety_WithInvalidId_ThrowsNotFoundException
CreateSociety_WithDuplicateName_ThrowsValidationException

// WRONG:
Test1
TestSociety
SocietyTest
```

---

## Best Practices

### Do

1. **Test behavior, not implementation**
2. **Use meaningful test data**
3. **Mock external dependencies**
4. **Test happy path, sad path, edge cases**

### Don't

1. **Don't test framework code**
2. **Don't make tests dependent on each other**
3. **Don't use Thread.Sleep in tests**
4. **Don't repeat test infrastructure**

---

## Running Tests

### Command Line

```bash
# .NET
dotnet test --configuration Release --verbosity normal

# Run specific test
dotnet test --filter "GetSociety_WithValidId_ShouldReturnSociety"

# Run with coverage
dotnet test /p:CollectCoverage=true

# JavaScript
npm test
npm run test:watch
npm run test:coverage
```

### CI/CD Integration

Tests run automatically on:
- Every PR
- Every push to develop
- Every push to main
- Before deployment

Test failures **block** deployment.
