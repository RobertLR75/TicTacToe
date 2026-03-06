# game-status-update-endpoint Specification

## Purpose
Defines unified game status update behavior, validation rules, PostgreSQL persistence semantics, and migration compatibility for legacy status routes.

## Requirements
### Requirement: Unified status update endpoint
The system SHALL expose a single status update endpoint for existing games that accepts `Active` and `Completed` as valid target statuses.

```json
{
  "status": "Active"
}
```

#### Scenario: Set game to Active
- **GIVEN** an existing game id
- **WHEN** the client sends a status update request with `status = Active`
- **THEN** the system SHALL return a successful response for the unified status endpoint

#### Scenario: Set game to Completed
- **GIVEN** an existing game id
- **WHEN** the client sends a status update request with `status = Completed`
- **THEN** the system SHALL return a successful response for the unified status endpoint

### Requirement: Status values are validated at the API boundary
The system MUST reject status update requests that do not specify `Active` or `Completed`.

#### Scenario: Invalid status is rejected
- **GIVEN** an existing game id
- **WHEN** the client sends a status update request with an unsupported status value
- **THEN** the system SHALL return a validation error response

#### Scenario: Missing status is rejected
- **GIVEN** an existing game id
- **WHEN** the client sends a status update request without a status field
- **THEN** the system SHALL return a validation error response

### Requirement: PostgreSQL status update uses game id as key
The system SHALL update game status in PostgreSQL using the provided game id as the row selector and MUST persist the requested status atomically.

#### Scenario: Existing game status is persisted by id
- **GIVEN** a persisted game row exists for the supplied game id
- **WHEN** a valid status update request is processed
- **THEN** PostgreSQL SHALL store the new status for that same game id

#### Scenario: Unknown game id is handled safely
- **GIVEN** no persisted game row exists for the supplied game id
- **WHEN** a valid status update request is processed
- **THEN** the system SHALL return a not-found response and MUST NOT create a new row implicitly

### Requirement: Backward compatibility during migration window
The system SHALL preserve compatibility for existing consumers of the legacy Complete and Activate routes during a migration window.

#### Scenario: Legacy Complete route maps to unified status update
- **GIVEN** a client calls the legacy Complete route during migration
- **WHEN** the request is processed
- **THEN** the system SHALL execute unified status update logic with `Completed`

#### Scenario: Legacy Activate route maps to unified status update
- **GIVEN** a client calls the legacy Activate route during migration
- **WHEN** the request is processed
- **THEN** the system SHALL execute unified status update logic with `Active`
