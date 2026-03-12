## Why

GameNotificationService currently crashes during startup when PostgreSQL is unreachable at process boot, even in local and Aspire-managed environments where dependency timing or developer setup may not be ready yet. We need startup behavior that is resilient enough to avoid an immediate hard crash while still making database availability problems visible and actionable.

## What Changes

- Update GameNotificationService startup so notification persistence initialization does not terminate the process immediately when PostgreSQL cannot be reached at boot.
- Introduce a controlled startup strategy for notification database migrations and repository readiness, with clear diagnostics when the database remains unavailable.
- Preserve the existing notification API surface and persistence model once PostgreSQL becomes reachable.
- Add or update tests covering startup behavior, migration execution, and unavailable-database handling.

## Capabilities

### New Capabilities
- `notification-persistence-startup-resilience`: GameNotificationService startup behavior for handling unreachable PostgreSQL dependencies during boot without an unhandled process crash.

### Modified Capabilities
- `notification-api`: Clarify notification service availability expectations when persistence dependencies are unavailable during startup, without changing the existing endpoint contract.

## Impact

- **Affected code**: `src/Backend/GameNotificationService` startup, persistence registration/migration flow, health/readiness behavior, and related tests.
- **Affected APIs**: No intended HTTP contract changes; any response differences should be limited to dependency-unavailable scenarios.
- **Affected systems**: Aspire AppHost local orchestration, GameNotificationService runtime boot path, PostgreSQL-dependent integration paths.
- **Affected teams**: Backend team (primary), frontend team consuming notifications, DevOps/local orchestration maintainers, QA/integration test maintainers.
- **Performance impact**: Negligible during normal operation; minor startup coordination or retry overhead may occur only while PostgreSQL is unavailable.
- **Rollback plan**: Revert the startup and migration orchestration changes to restore the current fail-fast boot behavior.
