# Code Review Guidelines

## Table of Contents
1. [PR Review Checklist](#pr-review-checklist)
2. [PR Submission Rules](#pr-submission-rules)
3. [What to Approve](#what-to-approve)
4. [What to Reject](#what-to-reject)
5. [Review Cycle](#review-cycle)

---

## PR Review Checklist

Use this checklist for EVERY pull request review:

### Design & Architecture

- [ ] Follows the documented architecture?
- [ ] No tight coupling between modules?
- [ ] Respects Clean Architecture layer separation?
- [ ] Database access only through repositories?
- [ ] No circular dependencies?
- [ ] Uses appropriate design patterns?

### Code Quality

- [ ] Code is readable and well-structured?
- [ ] Naming conventions followed?
- [ ] No code duplication (DRY principle)?
- [ ] Methods < 50 lines?
- [ ] Classes have single responsibility?
- [ ] Comments explain WHY, not WHAT?
- [ ] No commented-out code left behind?

### Performance

- [ ] No N+1 database queries?
- [ ] Efficient algorithms used?
- [ ] No unnecessary loops or iterations?
- [ ] API calls batched when possible?
- [ ] Caching implemented where needed?

### Security

- [ ] No hardcoded secrets/API keys?
- [ ] Input validation applied?
- [ ] SQL injection prevention (parameterized queries)?
- [ ] CORS properly configured?
- [ ] Authentication required for protected endpoints?
- [ ] Authorization checks in place?
- [ ] Sensitive data not logged?

### Testing

- [ ] Unit tests added for new logic?
- [ ] Tests don't depend on database?
- [ ] Mocks used appropriately?
- [ ] Edge cases covered?
- [ ] Tests have meaningful names?
- [ ] Negative test cases included?
- [ ] Code coverage maintained > 80%?

### Documentation

- [ ] Complex logic is documented?
- [ ] API endpoints documented (OpenAPI)?
- [ ] README updated if needed?
- [ ] Breaking changes noted?

### Formatting & Style

- [ ] Code formatted consistently?
- [ ] Follows coding standards document?
- [ ] No trailing whitespace?
- [ ] File encoding correct (UTF-8)?

---

## PR Submission Rules

### Size Limits

- **Maximum 400 lines of code** per PR
- **If larger**: Split into multiple PRs

### Branch Naming

```bash
# Feature
git checkout -b feature/society-management

# Bugfix
git checkout -b fix/billing-calculation-error

# Refactor
git checkout -b refactor/payment-service-simplification

# Chore
git checkout -b chore/update-dependencies
```

### Commit Message Format

```
[TYPE] Subject - max 50 chars

Optional detailed description explaining WHAT and WHY.

Valid types: feat, fix, refactor, docs, test, chore, perf
```

### PR Description Template

```markdown
## Description
What does this PR do? (1-2 sentences)

## Type of Change
- [ ] Feature
- [ ] Bug Fix
- [ ] Refactoring
- [ ] Documentation

## Testing
How was this tested? Include test commands/steps.

## Checklist
- [ ] Followed coding standards
- [ ] Added/updated tests
- [ ] Updated documentation
- [ ] No breaking changes
- [ ] Ready for production
```

---

## What to Approve

**Approve when**:

1. Code follows standards
2. Architecture is sound
3. Tests are comprehensive
4. No security issues
5. Performance is acceptable
6. At least one review completed

---

## What to Reject

**Request changes when**:

### 1. Hardcoded Values

```csharp
// REJECT THIS
const string ApiUrl = "https://api.production.com";
const string DatabasePassword = "sa123456";

// Request: Use environment variables
```

### 2. Business Logic in Controllers

```csharp
// REJECT THIS
[HttpPost("calculate")]
public ActionResult Calculate(int societyId) {
    // 50 lines of calculation logic here...
}

// Request: Move to service layer
```

### 3. No Tests

```csharp
// REJECT THIS
// Added new BillCalculator class with no tests

// Request: Add unit tests for BillCalculator
```

### 4. SQL Injection Risk

```csharp
// REJECT THIS
var query = $"SELECT * FROM Societies WHERE Name = '{name}'";
var result = _context.FromSqlRaw(query);

// Request: Use parameterized queries
```

### 5. Missing Security Checks

```csharp
// REJECT THIS
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteSociety(int id) {
    await _service.DeleteAsync(id);
    return Ok();
}

// Request: Add authorization attribute
[Authorize(Roles = "Admin")]
[HttpDelete("{id}")]
```

---

## Review Cycle

### Timeline

| Phase | Duration | Owner |
|-------|----------|-------|
| **Author develops** | - | Developer |
| **Author opens PR** | - | Developer |
| **First review** | <= 4 hours | Senior dev |
| **Author addresses feedback** | <= 24 hours | Developer |
| **Second review** | <= 2 hours | Different reviewer |
| **Approval & merge** | - | Any approver |

### Rules

1. **Minimum 1 approval required** before merge
2. **All conversations resolved** before merge
3. **CI/CD pipeline passes** before merge
4. **Automatic deletion** of branch after merge

---

## Review Tips for Reviewers

### Focus Areas (In Priority Order)

1. **Architecture** - Does it fit the design?
2. **Security** - Any vulnerabilities?
3. **Performance** - Any bottlenecks?
4. **Tests** - Adequate coverage?
5. **Style** - Consistent with standards?

### Constructive Feedback

**Bad Comment**:
```
This is wrong
```

**Good Comment**:
```
This approach might cause N+1 query issues.
Consider using Include() to preload related data.
See similar pattern in SocietyRepository.cs line 45.
```

---

## Automatic Checks (CI/CD Integration)

The following checks run automatically:

- Build succeeds
- All tests pass
- Code coverage > 80%
- No security vulnerabilities (SonarQube)
- Style compliance (ESLint/StyleCop)

**If any check fails**: PR cannot be merged until fixed.

---

## Escalation

If reviewer and author disagree:

1. **Technical lead** makes final decision
2. Decision documented in PR
3. Decision added to team wiki to prevent future disputes
