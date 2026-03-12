## ADDED Requirements

### Requirement: Startup initialization tolerates unavailable PostgreSQL
GameNotificationService SHALL handle notification persistence initialization failures caused by unreachable PostgreSQL without terminating the process with an unhandled exception.

#### Scenario: Service starts while PostgreSQL is unavailable
- **WHEN** GameNotificationService begins startup and PostgreSQL cannot be reached for notification migration or persistence initialization
- **THEN** the service SHALL remain running instead of crashing with an unhandled exception
- **AND** the service SHALL log that notification persistence initialization failed because PostgreSQL is unavailable

### Requirement: Persistence readiness is tracked explicitly
GameNotificationService SHALL maintain explicit notification persistence readiness state based on whether startup initialization and migrations have succeeded.

#### Scenario: Readiness remains unavailable after initialization failure
- **WHEN** notification persistence initialization fails during startup
- **THEN** the service SHALL mark notification persistence as not ready
- **AND** dependent runtime behavior SHALL use that readiness state instead of assuming PostgreSQL is available

#### Scenario: Readiness becomes available after successful initialization
- **WHEN** notification persistence initialization later succeeds
- **THEN** the service SHALL mark notification persistence as ready
- **AND** persistence-backed request handling SHALL become available without requiring a process restart

### Requirement: Initialization can recover after transient startup failure
GameNotificationService SHALL re-attempt notification persistence initialization after a transient PostgreSQL startup failure so that the service can recover automatically once the database becomes reachable.

#### Scenario: PostgreSQL becomes reachable after service start
- **WHEN** PostgreSQL was unavailable during initial startup but becomes reachable later
- **THEN** GameNotificationService SHALL re-attempt notification persistence initialization
- **AND** successful migration execution SHALL enable persistence-backed operations for subsequent requests
