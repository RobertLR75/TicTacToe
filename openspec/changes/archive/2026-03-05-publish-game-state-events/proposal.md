## Why

Game lifecycle changes currently stay inside the service boundary, so downstream systems cannot react to game creation or board updates in near real time. We need explicit domain event publishing now to enable integrations (analytics, notifications, and projections) using the existing RabbitMQ + MassTransit messaging stack.

## What Changes

- Publish a `GameCreatedEvent` when a new game is created by `GameStateService`.
- Publish a `GameStateUpdatedEvent` whenever game state changes are persisted by `GameStateService`.
- Configure MassTransit with RabbitMQ for event publication in the API host.
- Add contract definitions for the two events and ensure payloads include stable identifiers and state details needed by consumers.
- Add tests that verify events are emitted on successful create/update flows and not emitted for failed operations.

## Capabilities

### New Capabilities
- `game-state-events`: Publish integration events for game creation and game state updates via RabbitMQ using MassTransit.

### Modified Capabilities
- None.

## Impact

- Affected code: `GameStateService`, dependency injection/bootstrap configuration, and messaging contracts.
- Affected systems: RabbitMQ broker, current API service, and downstream consumers that subscribe to game events.
- Dependencies: MassTransit RabbitMQ transport configuration and related options binding/validation.
- Affected teams: API/platform team (producer ownership), integration/consumer teams (event adoption), DevOps team (broker config in each environment).
- Performance impact: small per-request overhead for publish operations; expected to be low but should be validated under load and monitored for broker latency/backpressure.
- Rollback plan: disable event publishing via configuration flag or remove publisher wiring and redeploy; keep service write behavior unchanged so core gameplay remains available.
