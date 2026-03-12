## Why

The solution now includes GameStateService and GameNotificationService, but local distributed orchestration needs to treat these services as first-class managed resources so developers can run, observe, and troubleshoot the full end-to-end flow from one AppHost entry point. Explicit orchestration requirements are needed to keep Aspire wiring consistent as services evolve.

## What Changes

- Extend Aspire AppHost orchestration requirements to include GameStateService and GameNotificationService as managed resources.
- Define required infrastructure/resource references and startup dependency behavior for both services within AppHost.
- Clarify frontend/backend service reference expectations so service discovery remains coherent in Aspire-managed local runs.

## Capabilities

### New Capabilities

### Modified Capabilities
- `aspire-frontend-orchestration`: Expand orchestration requirements beyond frontend+GameService so AppHost also manages GameStateService and GameNotificationService with the necessary references and startup sequencing.

## Impact

- Affected code: `src/TicTacToe.AppHost/AppHost.cs` and related AppHost configuration for service registration and references.
- Affected systems: Aspire local orchestration, service discovery, and startup health/dependency flow.
- Affected teams: Platform/DevOps maintainers of AppHost, backend service owners, and frontend developers running the full stack locally.
- Performance impact: Minimal runtime impact; startup may include additional managed resources but improves operational consistency.
- Rollback plan: Revert AppHost registration and reference changes to restore prior orchestration scope.
