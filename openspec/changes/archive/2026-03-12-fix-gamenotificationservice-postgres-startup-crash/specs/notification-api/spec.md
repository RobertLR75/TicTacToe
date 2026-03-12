## MODIFIED Requirements

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
