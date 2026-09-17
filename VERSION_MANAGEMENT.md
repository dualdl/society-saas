# Version Management Guide

Comprehensive guide for managing and updating versions across the Society SaaS platform.

## Overview

Centralized version tracking enables:
- Automated dependency updates
- Cross-component consistency
- Breaking change detection
- Compatibility matrix validation

---

## Backend Versioning

### .NET Framework

**Current**: .NET 10.0

To update:
1. Update `TargetFramework` in all `.csproj` files:
   ```xml
   <TargetFramework>net10.0</TargetFramework>
   ```

2. Update `global.json` (if exists):
   ```json
   {
     "sdk": {
       "version": "10.0.0"
     }
   }
   ```

3. Test: `dotnet build && dotnet test`

### NuGet Packages

**Current Key Packages**:
| Package | Version | Purpose |
|---------|---------|---------|
| EntityFrameworkCore | 10.x | ORM |
| FluentValidation | 11.x | Validation |
| MediatR | 12.x | CQRS |
| xUnit | 2.x | Testing |
| Moq | 4.x | Mocking |

To update:
```bash
# Update specific package
dotnet add package EntityFrameworkCore --version 10.0.0

# Update all packages
dotnet list package --outdated
# Then update each in .csproj
```

---

## Frontend Versioning

### React & TypeScript

**Current**: React 18+, TypeScript 5+

To update:
1. Update `package.json`:
   ```json
   {
     "dependencies": {
       "react": "^18.3.0",
       "react-dom": "^18.3.0"
     },
     "devDependencies": {
       "typescript": "^5.3.0"
     }
   }
   ```

2. Install: `npm install`

3. Test: `npm test`

### npm Packages

**Current Key Packages**:
| Package | Version | Purpose |
|---------|---------|---------|
| @mui/material | 5.x | UI Components |
| react-router-dom | 6.x | Routing |
| react-query | 3.x | Server State |
| axios | 1.x | HTTP Client |

To update:
```bash
# Check outdated
npm outdated

# Update specific
npm update @mui/material

# Update all
npm update
```

---

## Update Workflow

### Manual Update

#### 1. Identify What to Update

```bash
# Backend
dotnet list package --outdated

# Frontend
npm outdated
```

#### 2. Update Version Files

Update the relevant `.csproj` or `package.json` files.

#### 3. Test Changes

```bash
# Backend tests
cd backend
dotnet build
dotnet test

# Frontend tests
cd frontend/society-web
npm install
npm test
```

#### 4. Commit and Push

```bash
git add .
git commit -m "chore: update dependencies

- EntityFrameworkCore: 9.0.0 -> 10.0.0
- React: 18.2.0 -> 18.3.0

Tested: Backend Frontend"

git push origin develop
```

### Automated Update (CI/CD)

GitHub Actions workflow can run on schedule:
- Checks for outdated dependencies
- Creates Pull Request automatically
- Runs tests before PR creation

---

## Compatibility Matrix

| .NET Version | EF Core | Status |
|-------------|---------|--------|
| 10.0 | 10.x | Supported |
| 9.0 | 9.x | Supported |
| 8.0 | 8.x | LTS |

| React Version | TypeScript | Status |
|--------------|------------|--------|
| 18.x | 5.x | Supported |
| 17.x | 4.x | Legacy |

---

## Breaking Changes

Document migrations when major versions are released:

```
Version 2.0.0 (planned):
- .NET 10 migration
- Breaking API changes documented in MIGRATION_GUIDE.md
```

---

## Update Schedule

- **Patch Updates**: Applied automatically, no approval needed
- **Minor Updates**: Requires team review, 1 approval
- **Major Updates**: Requires breaking change assessment, 2 approvals

---

## Best Practices

**DO:**
- Always run tests after updating
- Create separate PR for each major component update
- Document breaking changes in commit message
- Update dependencies in order: Infrastructure -> Backend -> Frontend

**DON'T:**
- Update versions directly without testing
- Skip testing after updates
- Update multiple components without individual PRs
- Mix version updates with feature changes in PR

---

## Troubleshooting

### npm install fails after update

```bash
cd frontend/society-web
rm -r node_modules package-lock.json
npm install
```

### Tests fail after framework update

Check release notes for breaking changes:
- .NET: https://learn.microsoft.com/dotnet/core/whats-new/
- React: https://react.dev/blog

### Build fails after NuGet update

```bash
dotnet nuget locals all --clear
dotnet restore
dotnet build
```
