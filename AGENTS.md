# Available Agents

**Agents** are specialized AI personas that understand specific domains and workflows for the Society SaaS platform.

---

## Society SaaS Developer

**Purpose**: Build and extend society management features following enterprise patterns

### Capabilities
- Create new domain modules following Clean Architecture
- Implement Entity Framework Core DbContext and migrations
- Create REST API endpoints with Swagger docs
- Write unit and integration tests
- Build frontend components with Material UI
- Deploy to Azure Web Apps

### Usage
```
@microservice-developer Create a new BillingService module
@microservice-developer Add a new endpoint to SocietyController
@microservice-developer Generate tests for PaymentService
```

### Instructions Applied
- `.github/copilot-instructions.md`
- `docs/architecture/coding-standards.md`
- `docs/architecture/testing.md`

---

## Code Reviewer

**Purpose**: Review pull requests against architecture and standards

### Capabilities
- Check coding standard violations
- Verify Clean Architecture layer separation
- Validate 80%+ test coverage
- Detect security vulnerabilities
- Review API design compliance
- Check database patterns
- Suggest refactoring improvements

### Usage
```
@code-reviewer Review this PR for standards compliance
@code-reviewer Check for security issues only
@code-reviewer Verify test coverage
```

### Instructions Applied
- `.github/copilot-instructions.md`
- `docs/architecture/coding-standards.md`
- `docs/architecture/testing.md`

---

## How to Use Agents

### In VS Code Chat
```
# Mention the agent directly
@microservice-developer Create a new module

# Or assign to an issue
@github assign @microservice-developer to this issue
```

### With Slash Commands
```
# These trigger specific agents
/microservices          # Use microservice-developer
/review                 # Use code-reviewer
```

---

## Creating New Agents

To add a new agent:

1. **Create agent definition**
   ```
   .github/agents/my-agent.agent.md
   ```

2. **Define configuration**
   ```yaml
   name: my-agent
   description: My specialized agent
   model: claude-opus
   instructions:
     - .github/copilot-instructions.md
     - .github/instructions/my-instructions.md
   tools:
     - file-write
     - terminal
   ```

3. **Add to this list**
   - Add section in AGENTS.md
   - Document usage examples
   - Link to related skills/prompts

4. **Test it**
   ```
   @my-agent Your first task
   ```

---

## Related Documentation

- **Instructions**: `.github/instructions/`
- **Architecture**: `docs/architecture/architecture.md`
- **Coding Standards**: `docs/architecture/coding-standards.md`
- **API Guidelines**: `docs/api/api-guidelines.md`
- **Testing**: `docs/architecture/testing.md`
- **CI/CD**: `docs/architecture/ci-cd.md`

---

**Last Updated**: September 2026
