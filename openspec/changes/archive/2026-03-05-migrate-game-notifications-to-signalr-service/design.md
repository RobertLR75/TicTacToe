## Context

`GameStateService` currently contains notification-oriented behavior that belongs to real-time fan-out responsibilities, while `GameNotificationService` is intended to own SignalR delivery concerns. The requested change moves notification publishing ownership into `GameNotificationService`, wiring event consumers for `GameCreatedEvent` and `GameStateUpdatedEvent` to emit `GameCreatedNotification` and `GameStateUpdatedNotification` respectively, and aligns the web client SignalR connection to that service boundary.

This is a cross-cutting change spanning backend event consumers, SignalR publish contracts, and frontend subscription flow. Public REST APIs are unchanged, but real-time contracts and routing behavior must remain backwards compatible at the functional level (users still receive timely game creation and update events).

## Goals / Non-Goals

**Goals:**
- Centralize SignalR notification publishing in `GameNotificationService`.
- Ensure `GameCreatedEvent` consumption triggers `GameCreatedNotification` publication.
- Ensure `GameStateUpdatedEvent` consumption triggers `GameStateUpdatedNotification` publication.
- Update client SignalR connection/subscriptions to use `GameNotificationService` for game lifecycle updates.
- Preserve existing gameplay UX expectations (initial board hydration and move-driven board updates via notifications).

**Non-Goals:**
- Changing gameplay domain rules or move validation behavior.
- Introducing new REST endpoints or altering existing API response contracts.
- Redesigning hub authorization or introducing new security models.
- Reworking deployment topology beyond required endpoint/config updates.

## Decisions

### Decision: Notification ownership moves fully to `GameNotificationService`
`NotificationService` responsibilities currently hosted in `GameStateService` will be moved so SignalR fan-out has a single owner.

**Rationale:** This reduces coupling between state mutation and real-time delivery concerns, clarifies operational ownership, and simplifies future notification enhancements.

**Alternatives considered:**
- Keep publishing in `GameStateService` and proxy through `GameNotificationService`: rejected because it preserves split ownership and additional hop complexity.
- Duplicate publishing logic in both services during transition: rejected due to risk of duplicate client events.

### Decision: Publish notifications from event consumers in `NotificationGameService`
`NotificationGameService` consumers for `GameCreatedEvent` and `GameStateUpdatedEvent` will map message payloads to SignalR notification contracts and publish immediately.

**Rationale:** Event-driven publication keeps notification latency low and matches current asynchronous architecture using messaging infrastructure.

**Alternatives considered:**
- Periodic polling/projection for outbound notifications: rejected due to higher latency and additional storage/projection complexity.

### Decision: Client SignalR endpoint binding is updated to `GameNotificationService`
Frontend SignalR setup will target the `GameNotificationService` endpoint and subscribe to `GameCreatedNotification` and `GameStateUpdatedNotification` events.

**Rationale:** Client routing must align with service ownership to ensure events are received from the active publisher.

**Alternatives considered:**
- Maintain legacy endpoint with internal forwarding: rejected as transitional complexity without long-term value.

## Risks / Trade-offs

- **[Risk] Event naming mismatches between publisher and client handlers** -> **Mitigation**: Define and validate canonical event names in shared contracts and integration tests.
- **[Risk] Duplicate or missing notifications during cutover** -> **Mitigation**: Use a controlled deployment sequence and temporarily instrument publish/receive telemetry for correlation.
- **[Risk] Client connection configuration drift across environments** -> **Mitigation**: Centralize SignalR base URL/config keys and validate in environment smoke tests.
- **[Trade-off] Additional responsibility in `GameNotificationService`** -> **Mitigation**: Keep consumer/publisher classes cohesive and isolate mapping logic for maintainability.

## Migration Plan

1. Add/adjust event consumers in `GameNotificationService` for `GameCreatedEvent` and `GameStateUpdatedEvent` to publish corresponding SignalR notifications.
2. Remove/migrate notification publishing behavior from `GameStateService` so only one publisher remains active.
3. Update client SignalR connection target and handler registration to `GameNotificationService` event stream.
4. Verify end-to-end flow in local/integration environments: create game, receive created notification, make move, receive state-updated notification.
5. Deploy with telemetry monitoring on event consume/publish and client receive paths.
6. Rollback strategy: restore previous `GameStateService` notification publishing path and client endpoint configuration if notification delivery degrades.

## Open Questions

- Should event contract DTOs be shared through an existing shared library or duplicated with explicit mapping boundaries?
- Are there any non-browser clients currently bound to the legacy SignalR endpoint that require compatibility shims?
- Do we need temporary dual-publish feature flags for staged rollout, or is single-step cutover acceptable in this environment?
