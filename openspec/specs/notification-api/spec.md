## Requirements

### Requirement: GameNotificationService project exists
The solution SHALL contain a new ASP.NET Core project named `GameNotificationService` located at `src/Backend/GameNotificationService/`, targeting `net10.0`.

#### Scenario: Project is part of the solution
- **WHEN** a developer opens `TicTacToe.sln`
- **THEN** the `GameNotificationService` project SHALL appear under the `src/Backend/` solution folder

#### Scenario: Project builds successfully
- **WHEN** a developer runs `dotnet build` on the solution
- **THEN** the `GameNotificationService` project SHALL compile without errors

### Requirement: FastEndpoints is configured
The `GameNotificationService` SHALL use FastEndpoints for API routing and Swagger for API documentation, matching the pattern used by `GameStateService`.

#### Scenario: FastEndpoints processes requests
- **WHEN** the service starts
- **THEN** FastEndpoints middleware SHALL be registered in the request pipeline

#### Scenario: Swagger is available in development
- **WHEN** the service is running in the `Development` environment
- **THEN** the Swagger UI SHALL be accessible for API exploration

### Requirement: ServiceDefaults integration
The `GameNotificationService` SHALL reference `TicTacToe.ServiceDefaults` and call `AddServiceDefaults()` during startup to enable OpenTelemetry, health checks, and service discovery.

#### Scenario: Health endpoint responds when dependencies are ready
- **WHEN** a GET request is sent to `/health` after notification persistence initialization has succeeded
- **THEN** the service SHALL return a `200 OK` response indicating healthy status

#### Scenario: Health endpoint exposes persistence startup failure
- **WHEN** a GET request is sent to `/health` while notification persistence is unavailable because PostgreSQL could not be reached during startup
- **THEN** the service SHALL return an unhealthy result instead of terminating the process
- **AND** the unhealthy result SHALL indicate notification persistence dependency failure

#### Scenario: Service appears in Aspire dashboard
- **WHEN** the Aspire AppHost is running
- **THEN** the `GameNotificationService` SHALL appear in the dashboard with telemetry data (logs, traces, metrics)

### Requirement: Aspire AppHost registration
The Aspire AppHost SHALL register `GameNotificationService` as a project resource with a dedicated HTTP endpoint on port `5130`.

#### Scenario: AppHost references the project
- **GIVEN** the Aspire AppHost `AppHost.cs`
- **WHEN** the application starts
- **THEN** the `GameNotificationService` SHALL be registered via `AddProject<Projects.GameNotificationService>()` with an HTTP endpoint named `"gamenotificationservice-http"` on port `5130`

### Requirement: Stub notification endpoint
The service SHALL expose a `GET /api/notifications` endpoint that returns notification data when notification persistence is ready and a controlled dependency-unavailable response when notification persistence is unavailable.

#### Scenario: Notification list is returned when persistence is ready
- **WHEN** a GET request is sent to `/api/notifications` after notification persistence initialization has succeeded
- **THEN** the service SHALL return `200 OK` with the notification response payload
- **AND** the response content type SHALL be `application/json`

#### Scenario: Dependency unavailable response while persistence is not ready
- **WHEN** a GET request is sent to `/api/notifications` while notification persistence is unavailable because PostgreSQL has not been initialized successfully
- **THEN** the service SHALL return a service-unavailable response instead of surfacing a low-level database exception
- **AND** the response SHALL indicate that notification persistence is temporarily unavailable
