# Branching Strategy (Git Flow)

## Overview

This repository uses **Git Flow** model for branches and releases.

```
main (production)
  | merge after code review
  |
develop (staging)
  | merge feature/fix branches
  |
+--┴-------------------------+
|                          |
feature/*                  fix/*
hotfix/*                release/*
```

---

## Branch Types

### 1. `main` - Production Branch

**Purpose**: Production-ready code only

**Rules**:
- Protected (no direct push)
- Requires PR + code review
- Requires CI/CD pass
- Code coverage > 80%
- Automatically deploys to production

### 2. `develop` - Integration Branch

**Purpose**: Staging environment, integration point for features

**Rules**:
- Protected (no direct push)
- Requires PR + code review
- Requires CI/CD pass
- Automatically deploys to staging

### 3. `feature/*` - Feature Development

**Naming**: `feature/<description>`

**Examples**:
```bash
git checkout -b feature/billing-module
git checkout -b feature/milk-subscription
git checkout -b feature/csv-onboarding
```

**Lifecycle**:
```bash
# 1. Create from develop
git checkout develop
git pull origin develop
git checkout -b feature/billing-module

# 2. Develop locally
git add .
git commit -m "[feat] Add billing module"
git push origin feature/billing-module

# 3. Submit PR to develop
# Wait for code review and CI/CD

# 4. Merge (squash commits)
git push origin --delete feature/billing-module
```

### 4. `fix/*` - Bug Fix

**Naming**: `fix/<description>`

**Examples**:
```bash
git checkout -b fix/bill-calculation-error
git checkout -b fix/society-not-found
```

### 5. `hotfix/*` - Production Hotfix

**Naming**: `hotfix/<description>`

**Use When**: Critical bug in production

**Lifecycle**:
```bash
# 1. Create from main (NOT develop!)
git checkout main
git pull origin main
git checkout -b hotfix/critical-security-fix

# 2. Fix the issue
git add .
git commit -m "[fix] Critical security vulnerability"

# 3. Submit PR to BOTH main and develop

# 4. Tag version
git tag v1.2.1 -m "Critical security hotfix"
```

### 6. `release/*` - Release Preparation

**Naming**: `release/v<version>`

**Examples**:
```bash
git checkout -b release/v1.2.0
```

---

## PR (Pull Request) Workflow

### Step 1: Create Feature Branch

```bash
git checkout develop
git pull origin develop
git checkout -b feature/new-billing-module
```

### Step 2: Commit Changes

```bash
git add .
git commit -m "[feat] Implement new billing module

- Added BillCalculator service
- Implemented validation logic
- Added unit tests
- Added integration tests"

git push origin feature/new-billing-module
```

### Step 3: Open PR

**Title**: `[feat] Add billing module`

**Description**:
```markdown
## Description
Adds new billing module with support for multiple bill types.

## Changes
- Implemented BillCalculator service
- Added FluentValidation validators
- Created integration with Society service

## Tests Added
- 12 unit tests
- 5 integration tests
- Coverage: 88%

## Checklist
- [x] Followed coding standards
- [x] Added tests (coverage > 80%)
- [x] Updated documentation
- [x] No breaking changes
```

### Step 4: Code Review

- Reviewer reviews code
- Asks clarifying questions
- Requests changes if needed

### Step 5: Approval & Merge

Once 1+ approvals and CI/CD pass:

```bash
# Squash merge to avoid messy history
git checkout develop
git pull origin develop
git merge --squash feature/new-billing-module
git commit -m "[feat] Add billing module"
git push origin develop

# Delete feature branch
git push origin --delete feature/new-billing-module
```

---

## Commit Message Format

### Standard Format

```
<type>: <subject>

<body>

<footer>
```

### Types

```
feat        -> New feature
fix         -> Bug fix
refactor    -> Code refactoring (no functional change)
perf        -> Performance improvement
test        -> Test addition/modification
docs        -> Documentation
chore       -> Dependency update, config change
style       -> Code formatting (not logic)
ci          -> CI/CD configuration
```

### Examples

```bash
# Feature
git commit -m "[feat] Add billing calculation endpoint"

# Bug fix
git commit -m "[fix] Correct rounding error in tax calculation"

# Documentation
git commit -m "[docs] Update API guidelines with pagination example"
```

---

## Branch Protection Rules

### For `main` branch:

- Require pull request reviews (>=1 approval)
- Require code review from code owners
- Require status checks to pass (CI/CD)
- Require branches to be up to date before merge
- Require code coverage > 80%
- Dismiss stale pull request approvals
- Allow force pushes: NO
- Allow deletions: NO

### For `develop` branch:

- Require pull request reviews (>=1 approval)
- Require status checks to pass (CI/CD)
- Require branches to be up to date before merge
- Require code coverage > 75%
- Allow force pushes: NO
- Allow deletions: NO

---

## Emergency Processes

### Critical Production Bug

```bash
# 1. Create hotfix
git checkout main
git checkout -b hotfix/critical-bug

# 2. Fix issue with tests
git add .
git commit -m "[fix] Critical production bug"

# 3. Submit PR to main (expedited review)
# 4. Merge and tag
git tag v1.2.1

# 5. Merge to develop
git checkout develop
git merge hotfix/critical-bug
```

---

## Best Practices

**Do**:
- Create small, focused branches
- Commit frequently with meaningful messages
- Keep branches short-lived (ideally < 1 week)
- Keep commits atomic (one change per commit)
- Rebase before merging (keep history clean)
- Squash merge to main

**Don't**:
- Merge without code review
- Force push to shared branches
- Commit directly to main/develop
- Create branches with unclear names
- Leave stale branches around
- Commit secrets or credentials

---

## Common Commands

```bash
# Show all branches
git branch -a

# Show current branch
git branch --show-current

# Delete local branch
git branch -d feature/branch-name

# Delete remote branch
git push origin --delete feature/branch-name

# Fetch latest from remote
git fetch --all

# Rebase current branch on develop
git rebase origin/develop

# Show commit history
git log --oneline --graph --all
```
