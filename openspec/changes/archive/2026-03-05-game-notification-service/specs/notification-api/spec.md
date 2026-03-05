## ADDED Requirements

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

#### Scenario: Health endpoint responds
- **WHEN** a GET request is sent to `/health`
- **THEN** the service SHALL return a `200 OK` response indicating healthy status

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
The service SHALL expose a stub `GET /api/notifications` endpoint that returns an empty JSON array, serving as a placeholder for future notification retrieval.

#### Scenario: Empty notification list
- **WHEN** a GET request is sent to `/api/notifications`
- **THEN** the service SHALL return `200 OK` with an empty JSON array `[]`
- **AND** the response content type SHALL be `application/json`
