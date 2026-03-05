## ADDED Requirements

### Requirement: Aspire orchestrates TicTacToeMud
The AppHost SHALL register TicTacToeMud as an Aspire-managed project so that it starts, stops, and is monitored alongside GameService.

#### Scenario: Frontend starts with Aspire
- **WHEN** the AppHost is launched
- **THEN** TicTacToeMud SHALL start as a managed resource alongside GameService

#### Scenario: Frontend references backend
- **WHEN** TicTacToeMud is registered in the AppHost
- **THEN** it SHALL hold a reference to the `gameservice` resource for future service discovery

### Requirement: TicTacToeMud uses ServiceDefaults
TicTacToeMud SHALL use the shared ServiceDefaults library to gain OpenTelemetry, health checks, and service discovery — matching the pattern used by GameService.

#### Scenario: ServiceDefaults are applied
- **WHEN** TicTacToeMud starts
- **THEN** OpenTelemetry tracing, metrics, and logging SHALL be configured
- **AND** health check endpoints (`/health`, `/alive`) SHALL be available
