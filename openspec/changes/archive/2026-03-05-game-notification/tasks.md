## 1. Messaging and Configuration

- [x] 1.1 Add MassTransit RabbitMQ consumer registration in `GameNotificationService` for `GameCreated` and `GameStateUpdated` events.
- [x] 1.2 Add strongly typed messaging configuration and startup validation for required broker settings.
- [x] 1.3 Add a feature toggle to enable/disable notification event processing per environment.

## 2. Notification Persistence

- [x] 2.1 Create notification persistence model/entity including event identity, game identifier, type, payload summary, and timestamps.
- [x] 2.2 Add FluentMigrator migration(s) for notification storage schema and required indexes.
- [x] 2.3 Implement idempotency guard logic to prevent duplicate records for duplicate event deliveries.

## 3. Consumer Processing

- [x] 3.1 Implement `GameCreated` consumer mapping incoming event data to persisted notification records.
- [x] 3.2 Implement `GameStateUpdated` consumer mapping incoming event data to persisted notification records.
- [x] 3.3 Add failure-path diagnostics for invalid event payloads without persisting malformed notifications.

## 4. Notification API Update

- [x] 4.1 Replace stub `GET /api/notifications` handler with persistence-backed retrieval.
- [x] 4.2 Add pagination support and bounded defaults for notification list queries.
- [x] 4.3 Add optional game identifier filter for `GET /api/notifications`.

## 5. Verification and Rollout

- [x] 5.1 Add unit tests for consumer mapping, idempotency behavior, and toggle-on/toggle-off processing.
- [x] 5.2 Add integration tests (RabbitMQ/TestContainers + database) verifying event-to-notification end-to-end flow.
- [x] 5.3 Verify existing notification route compatibility and document rollout/rollback runbook steps.
