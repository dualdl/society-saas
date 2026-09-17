# Copilot Customization Quick Reference

**For**: AI Agents cloning this repository
**Purpose**: Understand what instructions, rules, and workflows apply

---

## What Gets Loaded (Order Matters!)

When any agent starts a task in this repository:

```
1. ALWAYS -> .github/copilot-instructions.md
   |
2. CONTEXT -> Read docs (architecture.md, coding-standards.md)
   |
3. IF FILE PATTERN MATCHES:
   +-- Creating .cs files -> docs/architecture/coding-standards.md
   +-- Creating test files -> docs/architecture/testing.md
   +-- Creating API files -> docs/api/api-guidelines.md
   |
4. IF TASK TYPE MATCHES:
   +-- @microservice-developer -> AGENTS.md
   +-- @code-reviewer -> docs/architecture/code-review.md
   |
5. EXECUTE -> Follow all loaded rules
   |
6. VERIFY -> Run tests and checks
```

---

## Key Rules (Always Enforced)

### Architecture
- Clean Architecture: Domain -> Application -> Infrastructure -> API
- Repository pattern for database access
- Dependency injection in constructors

### Code Style
- PascalCase: `SocietyService`, `ISocietyRepository`
- camelCase: `societyName`, `pageSize`
- Constants: `MAX_RETRIES`, `DATABASE_KEY`
- SOLID principles: Single responsibility, no god objects

### Async/Await
- Use `async Task<T>` (method name ends with `Async`)
- Never `.Result` or `.Wait()`
- `await` every async operation
- `ToListAsync()`, `SaveChangesAsync()`

### Testing
- Minimum 80% coverage
- Unit + Integration + E2E tests
- AAA pattern: Arrange, Act, Assert
- Cannot merge if coverage < 80%

### Database
- Entity Framework Core only
- Migrations tracked in git
- All operations async
- Soft deletes for audit trail

### Security
- No hardcoded secrets
- No SQL injection
- Use Key Vault for secrets
- Authentication on sensitive endpoints

### API
- `/api/v1/` versioning
- REST conventions: GET, POST, PUT, DELETE
- Response format: `{ success, data, message, timestamp }`
- Swagger documentation required

---

## File Locations Reference

| Need | File | Location |
|------|------|----------|
| General rules | copilot-instructions.md | `.github/` |
| Architecture | architecture.md | `docs/architecture/` |
| Coding standards | coding-standards.md | `docs/architecture/` |
| API guidelines | api-guidelines.md | `docs/api/` |
| Testing rules | testing.md | `docs/architecture/` |
| Code review | code-review.md | `docs/architecture/` |
| Branching strategy | branching-strategy.md | `docs/architecture/` |
| Troubleshooting | troubleshooting.md | `docs/architecture/` |
| CI/CD pipeline | ci-cd.md | `docs/architecture/` |
| Available agents | AGENTS.md | Root |
| New module guide | NEW_SERVICE_GUIDE.md | Root |

---

## Common Workflows

### 1. Create New Module Feature
```
User: "@microservice-developer Add BillingService"
|
Agent: Loads AGENTS.md
Agent: Loads coding-standards.md
Agent: Loads NEW_SERVICE_GUIDE.md
Agent: Creates domain entities, services, controller
Agent: Generates tests
Output: New feature with tests
Result: Code compiles, tests pass, 80%+ coverage
```

### 2. Review Pull Request
```
User: "@code-reviewer Review this PR"
|
Agent: Loads code-review.md
Agent: Loads coding-standards.md
Agent: Loads testing.md
Agent: Analyzes PR diff
Output: Review comment with checklist
Result: Approve/Request Changes/Comment
```

### 3. Add Feature to Existing Module
```
User: "Add UpdateSociety endpoint"
|
Agent: Loads copilot-instructions.md
Agent: Loads coding-standards.md
Agent: Adds feature following rules
Agent: Generates tests
Output: Updated module with tests
Result: Code compiles, tests pass
```

---

## Agent Decision Tree

```
User asks agent to do something...

+-- Is it about modules/features?
|   +-- YES -> Load coding-standards.md
|   +-- NO  -> Check other patterns
|
+-- Is it a search/analysis task?
|   +-- YES -> Can use @code-reviewer
|   +-- NO  -> Continue
|
+-- Is it building something new?
|   +-- YES -> Use @microservice-developer
|   +-- NO  -> Check other patterns
|
+-- Is it a PR review?
|   +-- YES -> Use @code-reviewer
|   +-- NO  -> Use default behavior
```

---

## When Things Go Wrong

### Build fails
1. Check: `dotnet build` output
2. Read: `docs/architecture/coding-standards.md`
3. Fix: Follow architecture rules
4. Retry: Ask agent to fix

### Tests fail
1. Check: `dotnet test` output
2. Read: `docs/architecture/testing.md`
3. Fix: Add missing tests or fix code
4. Retry: Ask agent to fix

### Coverage < 80%
1. Check: Coverage report
2. Read: `docs/architecture/testing.md`
3. Fix: Add more tests
4. Blocker: Cannot merge until fixed

### Security issue
1. Check: Hardcoded secrets?
2. Read: `docs/architecture/coding-standards.md` security section
3. Fix: Use Key Vault
4. Blocker: Cannot merge until fixed

---

## Learning Path

**First Time?** Read in this order:

1. **This file** (you are here) - Understand the system
2. `docs/architecture/architecture.md` - System design
3. `docs/architecture/coding-standards.md` - Code style
4. `docs/api/api-guidelines.md` - API design
5. `NEW_SERVICE_GUIDE.md` - How to build a module
6. `docs/architecture/testing.md` - Testing strategy
7. `docs/architecture/branching-strategy.md` - Git workflow

---

**Version**: 1.0
**Last Updated**: September 2026
**Maintained By**: Engineering Team
