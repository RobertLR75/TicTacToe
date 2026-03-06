## Why

The current GameService exposes separate Complete and Activate endpoints for status transitions, which duplicates API surface area and persistence logic. Consolidating these actions into a single update endpoint reduces client complexity and creates a clearer contract for status lifecycle changes.

## What Changes

- Replace separate Complete and Activate endpoints with one status update endpoint that accepts `Active` or `Completed`.
- Update request handling to route status transitions through a single GameService update flow.
- Ensure PostgreSQL updates correctly target the game identifier and persist the requested status.
- Maintain backward compatibility strategy for existing consumers through deprecation messaging and migration guidance.

## Capabilities

### New Capabilities
- `game-status-update-endpoint`: Unified API behavior for updating a game's status to `Active` or `Completed` through one endpoint.

### Modified Capabilities
- None.

## Impact

- Affected code: GameService endpoint definitions, request/response DTOs, status transition handlers, PostgreSQL persistence queries/repositories, API docs.
- Affected APIs: Existing Complete/Activate endpoints deprecated and replaced by a unified update endpoint.
- Dependencies/systems: FastEndpoints routing, Entity Framework Core or Dapper persistence paths, PostgreSQL schema/data access consistency checks.
- Affected teams: Backend API team, frontend/client team, QA/integration testing team, DevOps/release team.
- Performance impact: Slightly improved routing and maintenance overhead by reducing endpoint branching; no expected material runtime regression.
- Rollback plan: Re-enable Complete/Activate routes and previous status transition handlers, restore prior persistence path, and redeploy with compatibility config if client migration issues arise.
