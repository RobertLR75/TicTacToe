## Why

Notification handling for game lifecycle events is currently split across `GameStateService`, which obscures ownership and makes SignalR notification flow harder to reason about and evolve. Consolidating notification responsibilities in a dedicated `GameNotificationService` now reduces coupling and prepares the codebase for consistent real-time event fan-out.

## What Changes

- Move `NotificationService` responsibilities out of `GameStateService` into `GameNotificationService`.
- Publish SignalR `GameCreatedNotification` when `GameCreatedEvent` is consumed by `NotificationGameService`.
- Publish SignalR `GameStateUpdatedNotification` when `GameStateUpdatedEvent` is consumed by `NotificationGameService`.
- Update the client SignalR connection and subscription flow to consume notifications from `NotificationGameService`.

## Capabilities

### New Capabilities
- `game-realtime-notifications`: Dedicated service-owned real-time game notifications for created and updated events delivered over SignalR.

### Modified Capabilities
- `game-page`: Client SignalR wiring and event handling requirements are updated to consume `NotificationGameService` notifications.

## Impact

- **Affected code**: Notification domain/service classes, game event consumers, SignalR hub publishing paths, and frontend SignalR client integration.
- **APIs and contracts**: SignalR event contracts for `GameCreatedNotification` and `GameStateUpdatedNotification`; no REST API contract change expected.
- **Dependencies and systems**: Messaging consumers remain event-driven; SignalR infrastructure and client subscriptions are primary integration points.
- **Affected teams**: Backend platform/game services team and frontend web client team.
- **Performance impact**: Minor additional notification publish operations on event consumption; expected low overhead, with monitoring needed for connection fan-out under high game update volume.
- **Rollback plan**: Revert to previous notification routing by restoring `NotificationService` behavior in `GameStateService`, disabling new publish calls in `NotificationGameService`, and rolling back the client SignalR target/service binding.
