## MODIFIED Requirements

### Requirement: Stub notification endpoint
The service SHALL expose `GET /api/notifications` as a data-backed endpoint that returns persisted notifications, replacing the placeholder empty-array behavior.

#### Scenario: Notification list returns persisted data
- **WHEN** a GET request is sent to `/api/notifications`
- **THEN** the service SHALL return `200 OK` with a JSON array of notification items ordered from newest to oldest
- **AND** the response content type SHALL be `application/json`

#### Scenario: Notification list is empty when no records exist
- **WHEN** a GET request is sent to `/api/notifications` and no notifications are stored
- **THEN** the service SHALL return `200 OK` with an empty JSON array `[]`

## ADDED Requirements

### Requirement: Notification query supports pagination and filters
The service SHALL support bounded notification retrieval using query parameters for pagination and optional filtering.

#### Scenario: Client requests paged notifications
- **WHEN** a client calls `/api/notifications` with pagination parameters
- **THEN** the service SHALL return a bounded page of notifications according to configured limits

#### Scenario: Client filters by game identifier
- **WHEN** a client calls `/api/notifications` with a valid game identifier filter
- **THEN** the service SHALL return only notifications matching that game identifier
