## 1. Backend Notification Ownership

- [x] 1.1 Locate and remove `NotificationService` notification publishing responsibilities from `GameStateService`.
- [x] 1.2 Move or re-home notification publishing logic into `GameNotificationService` with clear service boundaries.
- [x] 1.3 Update dependency injection and project references so notification publishing resolves only through `GameNotificationService`.

## 2. Event Consumer to SignalR Publishing

- [x] 2.1 Update `NotificationGameService` consumer for `GameCreatedEvent` to publish `GameCreatedNotification` to the correct SignalR audience.
- [x] 2.2 Update `NotificationGameService` consumer for `GameStateUpdatedEvent` to publish `GameStateUpdatedNotification` to the correct SignalR audience.
- [x] 2.3 Add/adjust contract mapping and validation to prevent malformed notifications from being published.
- [x] 2.4 Add logging/telemetry around consume-to-publish flow for both event types.

## 3. Client SignalR Integration

- [x] 3.1 Update client SignalR connection configuration to target `GameNotificationService`.
- [x] 3.2 Update client event handlers to subscribe to `GameCreatedNotification` and `GameStateUpdatedNotification`.
- [x] 3.3 Ensure game page state update flow (initial load and move updates) uses the renamed notifications without changing gameplay behavior.

## 4. Validation and Regression Coverage

- [x] 4.1 Add or update unit tests for `NotificationGameService` event consumers and notification mapping behavior.
- [x] 4.2 Add or update integration tests covering create-game and move flows to verify SignalR notifications are published and consumed end-to-end.
- [x] 4.3 Run solution build and relevant backend/frontend test suites; fix any regressions introduced by service and event name changes.
