## Why

Players currently have no reliable way to see game-related notifications after game events occur, which limits product engagement and operational visibility. Implementing a first real game notification flow now enables the existing `GameNotificationService` to move from a placeholder API into a usable feature.

## What Changes

- Add a notification ingestion path for game lifecycle events (`GameCreated`, `GameStateUpdated`) into `GameNotificationService`.
- Persist normalized notification records for later retrieval rather than returning a hardcoded empty list.
- Replace the stub `GET /api/notifications` behavior with real retrieval of recent notifications.
- Add basic filtering and pagination for notification reads to keep responses bounded.
- Add rollout/rollback controls so event-driven notification processing can be disabled safely if needed.

## Capabilities

### New Capabilities
- `game-notification-processing`: Process game lifecycle events into stored notification records that can be queried by the notification API.

### Modified Capabilities
- `notification-api`: Update API requirements from stub-only response to real notification retrieval behavior, preserving backward compatibility.

## Impact

- Affected code: `GameNotificationService` consumers, persistence layer, and notification endpoint implementation.
- APIs/contracts: Existing `GET /api/notifications` endpoint remains available but changes from static empty response to data-backed output.
- Dependencies/systems: RabbitMQ + MassTransit ingestion path, database persistence, and AppHost/service configuration.
- Performance impact analysis: Adds background event handling plus read-path database queries; expected low to moderate overhead controlled through indexing and pagination defaults.
- Affected teams: Backend service owners, game-state event producers, QA/integration testing, and DevOps (messaging/database configuration).
- Rollback plan: Disable event ingestion through configuration toggle and revert endpoint to stub or previous deployment while preserving existing public route shape.
