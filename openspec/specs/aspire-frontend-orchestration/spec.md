# aspire-frontend-orchestration Specification

## Purpose
Aspire orchestration of the TicTacToeMud frontend, providing managed lifecycle, service discovery, OpenTelemetry, and health checks.

## Requirements

### Requirement: Aspire orchestrates TicTacToeMud
The AppHost SHALL register TicTacToeMud as an Aspire-managed project so that it starts, stops, and is monitored alongside GameService, GameStateService, and GameNotificationService.

#### Scenario: Frontend starts with Aspire
- **WHEN** the AppHost is launched
- **THEN** TicTacToeMud SHALL start as a managed resource alongside GameService, GameStateService, and GameNotificationService

#### Scenario: Frontend references backend services
- **WHEN** TicTacToeMud is registered in the AppHost
- **THEN** it SHALL hold a reference to the `gamestateservice` resource for gameplay API/service discovery integration
- **AND** it SHALL hold a reference to the `gamenotificationservice` resource for real-time notification integration

### Requirement: TicTacToeMud uses ServiceDefaults
TicTacToeMud SHALL use the shared ServiceDefaults library to gain OpenTelemetry, health checks, and service discovery — matching the pattern used by GameService.

#### Scenario: ServiceDefaults are applied
- **WHEN** TicTacToeMud starts
- **THEN** OpenTelemetry tracing, metrics, and logging SHALL be configured
- **AND** health check endpoints (`/health`, `/alive`) SHALL be available

### Requirement: AppHost orchestrates GameStateService with required resources
The AppHost SHALL register GameStateService as an Aspire-managed project with explicit references to required infrastructure resources used during runtime.

#### Scenario: GameStateService registration includes dependencies
- **WHEN** GameStateService is configured in AppHost
- **THEN** it SHALL be registered as the `gamestateservice` project resource
- **AND** it SHALL reference Redis, PostgreSQL, and RabbitMQ resources required for runtime operations

### Requirement: AppHost orchestrates GameNotificationService with startup dependency sequencing
The AppHost SHALL register GameNotificationService as an Aspire-managed project and enforce startup sequencing for its hard dependencies.

#### Scenario: Notification service waits for dependencies
- **WHEN** GameNotificationService is configured in AppHost
- **THEN** it SHALL be registered as the `gamenotificationservice` project resource
- **AND** it SHALL reference PostgreSQL and RabbitMQ resources
- **AND** it SHALL wait for PostgreSQL and RabbitMQ before service startup is considered ready
