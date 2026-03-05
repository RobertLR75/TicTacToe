## 1. Messaging Setup

- [x] 1.1 Add MassTransit RabbitMQ registration in `GameNotificationService` startup/composition root.
- [x] 1.2 Bind and validate RabbitMQ configuration options required for consumer endpoint startup.
- [x] 1.3 Configure explicit receive endpoint names for `GameCreated` and `GameStateUpdated` consumers.

## 2. Consumer Implementation

- [x] 2.1 Create `GameCreated` consumer class and wire message contract handling.
- [x] 2.2 Create `GameStateUpdated` consumer class and wire message contract handling.
- [x] 2.3 Implement bootstrap handling behavior in both consumers using `Console.WriteLine` only.

## 3. Integration Verification

- [x] 3.1 Add automated tests for consumer registration and startup failure on invalid/missing messaging configuration.
- [x] 3.2 Add integration tests (RabbitMQ/TestContainers where available) to verify both event types are consumed.
- [x] 3.3 Verify existing `GameNotificationService` REST endpoints remain backward compatible after consumer wiring.

## 4. Rollout and Documentation

- [x] 4.1 Document queue names, required RabbitMQ settings, and local run instructions for event consumption.
- [x] 4.2 Document rollback steps to disable or remove consumer endpoint registration safely.
