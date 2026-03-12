## Why

GameService currently crashes at startup when `ConnectionStrings:postgres` is not present, which prevents local development and orchestration scenarios from booting the service reliably. We need startup behavior that works with the repository's intended local/Aspire configuration model instead of requiring a manually supplied connection string in every environment.

## What Changes

- Fix GameService startup configuration so persistence can resolve PostgreSQL settings from the supported local/Aspire configuration sources.
- Ensure GameService fails with a clearer, actionable startup message only when required persistence configuration is truly unavailable.
- Align configuration loading with existing service orchestration and local development patterns used in the solution.
- Add or update tests for persistence configuration resolution and startup validation behavior.
- Keep public API behavior unchanged.

## Capabilities

### New Capabilities
- `gameservice-persistence-configuration`: GameService startup configuration for resolving PostgreSQL persistence settings from supported environment and orchestration sources.

### Modified Capabilities
- `game-persistence`: Update persistence requirements so GameService startup can resolve required PostgreSQL configuration through supported runtime configuration sources without brittle manual setup.

## Impact

- **Affected code**: `src/Backend/GameService` startup and persistence registration, related configuration handling, and associated tests.
- **Affected APIs**: No HTTP/API contract changes.
- **Affected teams**: Backend team (primary), DevOps/Aspire orchestration maintainers, QA/integration test maintainers.
- **Performance impact**: Negligible; configuration resolution happens only at startup.
- **Rollback plan**: Revert the startup configuration changes and restore the previous strict `ConnectionStrings:postgres` requirement if needed.
