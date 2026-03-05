## 1. Messaging Configuration

- [x] 1.1 Add MassTransit RabbitMQ configuration and strongly typed settings binding in the API composition root.
- [x] 1.2 Add startup validation/health checks so invalid RabbitMQ configuration fails fast with actionable errors.
- [x] 1.3 Add feature/config toggle support to enable or disable event publishing by environment.

## 2. Event Contracts

- [x] 2.1 Create `GameCreatedEvent` contract with required fields (game ID, initial state details, timestamp, correlation metadata).
- [x] 2.2 Create `GameStateUpdatedEvent` contract with required fields (game ID, updated state details, timestamp, correlation metadata).
- [x] 2.3 Place contracts in the agreed namespace/project location and document versioning expectations.

## 3. GameStateService Publishing

- [x] 3.1 Inject `IPublishEndpoint` (or approved publisher abstraction) into `GameStateService`.
- [x] 3.2 Publish `GameCreatedEvent` only after successful game creation persistence.
- [x] 3.3 Publish `GameStateUpdatedEvent` only after successful game update persistence.
- [x] 3.4 Ensure failed create/update operations do not publish events.

## 4. Verification and Compatibility

- [x] 4.1 Add unit tests covering success/failure publish behavior for create and update flows.
- [x] 4.2 Add integration tests (with RabbitMQ/TestContainers if available) to verify broker publication wiring.
- [x] 4.3 Verify existing REST create/update request and response contracts remain backward compatible.
- [x] 4.4 Add operational notes for rollout and rollback (toggle-off path) in change documentation.
