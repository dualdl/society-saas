# CI/CD Pipeline

## Table of Contents
1. [Pipeline Overview](#pipeline-overview)
2. [GitHub Actions Workflows](#github-actions-workflows)
3. [Branch Strategy](#branch-strategy)
4. [Deployment Environments](#deployment-environments)
5. [Quality Gates](#quality-gates)

---

## Pipeline Overview

### Complete Flow

```
Developer Push
    |
Git Webhook -> GitHub Actions
    |
+---------------------------------+
| CI: Build & Test                |
+---------------------------------+
| - Restore NuGet packages        |
| - Build projects                |
| - Run unit tests                |
| - Run integration tests         |
| - Code coverage analysis        |
| - Security scanning             |
+---------------------------------+
    | (if main branch)
+---------------------------------+
| CD: Deploy to Production        |
+---------------------------------+
| - Build publish artifact        |
| - Deploy API to App Service     |
| - Build & Deploy Frontend       |
| - Run smoke tests               |
+---------------------------------+
    |
Production Live
```

---

## GitHub Actions Workflows

### 1. Pull Request CI Workflow

**File**: `.github/workflows/pr-ci.yml`

```yaml
name: PR - Build & Test

on:
  pull_request:
    branches: [ develop, main ]

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '10.0'

    - name: Restore NuGet packages
      run: dotnet restore

    - name: Build
      run: dotnet build --configuration Release --no-restore

    - name: Run unit tests
      run: dotnet test --configuration Release --no-build --verbosity normal

    - name: Code coverage
      run: dotnet test --configuration Release --collect:"XPlat Code Coverage"
```

### 2. Push to Develop (Staging)

**File**: `.github/workflows/push-develop.yml`

```yaml
name: Develop - Build & Deploy to Staging

on:
  push:
    branches: [ develop ]

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '10.0'

    - name: Restore
      run: dotnet restore

    - name: Build
      run: dotnet build --configuration Release --no-restore

    - name: Tests
      run: dotnet test --configuration Release --no-build

    - name: Publish API
      run: dotnet publish backend/SocietySaaS.API/SocietySaaS.API.csproj -c Release -o ./publish/api

    - name: Deploy API to Staging
      uses: azure/webapps-deploy@v2
      with:
        app-name: society-saas-api-staging
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE_API_STAGING }}
        package: ./publish/api

    - name: Build Frontend
      run: |
        cd frontend/society-web
        npm install
        npm run build

    - name: Deploy Frontend to Staging
      uses: azure/webapps-deploy@v2
      with:
        app-name: society-saas-web-staging
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE_WEB_STAGING }}
        package: frontend/society-web/build
```

### 3. Push to Main (Production)

**File**: `.github/workflows/push-main.yml`

```yaml
name: Main - Build & Deploy to Production

on:
  push:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '10.0'

    - name: Build & Test
      run: |
        dotnet restore
        dotnet build --configuration Release
        dotnet test --configuration Release

    - name: Publish API
      run: dotnet publish backend/SocietySaaS.API/SocietySaaS.API.csproj -c Release -o ./publish/api

    - name: Deploy API to Production
      uses: azure/webapps-deploy@v2
      with:
        app-name: society-saas-api
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE_API }}
        package: ./publish/api

    - name: Build Frontend
      run: |
        cd frontend/society-web
        npm install
        npm run build

    - name: Deploy Frontend to Production
      uses: azure/webapps-deploy@v2
      with:
        app-name: society-saas-web
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE_WEB }}
        package: frontend/society-web/build

    - name: Notify Slack
      if: always()
      uses: slackapi/slack-github-action@v1
      with:
        webhook-url: ${{ secrets.SLACK_WEBHOOK }}
        payload: |
          {
            "text": "Production deployment ${{ job.status }}",
            "channel": "#deployments"
          }
```

---

## Branch Strategy

### Git Flow Model

```
main (production)
  ^
  +-- pull_request (code review)

develop (staging)
  ^
  +-- feature/billing-module
  +-- fix/bill-calculation-error
  +-- refactor/society-service
```

### Branch Rules

| Branch | Protection | Auto-Deploy | Role |
|--------|-----------|------------|------|
| `main` | Yes | Production | Production releases |
| `develop` | Yes | Staging | Integration branch |
| `feature/*` | No | No | Feature development |
| `hotfix/*` | Yes | Production | Emergency fixes |

### Protection Rules

**Main branch**:
- Require pull request reviews (>=1)
- Require status checks to pass (CI/CD)
- Require code coverage > 80%
- Dismiss stale reviews
- Restrict who can push

**Develop branch**:
- Require pull request reviews (>=1)
- Require status checks to pass
- Require code coverage > 75%

---

## Deployment Environments

### Development (Developer Machine)

```
Developer runs locally
-> No CI/CD needed
-> Manual testing
```

### Integration (develop branch -> Staging)

```
Develop push
  |
CI builds all
  |
Tests pass?
  | Yes
Deploy to Azure App Service Staging
  |
Manual testing by QA
```

### Production (main branch)

```
Main push
  |
Full CI pipeline
  |
All checks pass?
  | Yes
Deploy to Azure App Service Production
  |
Smoke tests & monitoring
  |
Live
```

---

## Quality Gates

### Build Must Pass

- Build succeeds
- All NuGet dependencies restored
- .NET version compatibility

### Tests Must Pass

- All unit & integration tests pass
- Coverage requirement: >= 80%
- No flaky tests

### Code Quality

- SonarQube analysis
- Code smells: < 10
- Bugs: 0
- Vulnerabilities: 
-
- must pass





Must#########-#########
 must-}

 

