## Context

GameStateService currently exposes FastEndpoints for game creation, read, and move operations, with orchestration logic split between endpoint and service classes. This change introduces an internal request/handler pattern (`IRequest<TResponse>` with `HandleAsync`) to make operation boundaries explicit, improve unit testability of individual flows, and align endpoint behavior across features.

Constraints:
- Keep public REST behavior backward compatible (payloads, status codes, and endpoint routes).
- Preserve existing integrations (event publishing via MassTransit, persistence and cache dependencies).
- Keep refactor bounded to GameStateService internals and existing tests unless parity gaps are discovered.

Stakeholders:
- Backend API maintainers (primary implementers and owners)
- Frontend/API consumers (depend on stable API behavior)

## Goals / Non-Goals

**Goals:**
- Establish a reusable request contract for GameStateService commands/queries.
- Move orchestration out of endpoint classes into dedicated handlers with `HandleAsync`.
- Keep endpoint contracts and observable behavior unchanged.
- Add/adjust tests to cover handler behavior and endpoint parity.

**Non-Goals:**
- Rewriting domain/game rules logic.
- Changing API routes, request/response schemas, or success/error semantics.
- Introducing a full mediator framework dependency for this refactor.

## Decisions

1. Introduce lightweight in-service request abstractions.
   - Decision: Add internal interfaces for `IRequest<TResponse>` and matching handler contract (`IRequestHandler<TRequest, TResponse>` with `HandleAsync`).
   - Rationale: Keeps pattern explicit without adding external mediator package complexity.
   - Alternative considered: Use MediatR directly.
   - Why not chosen: Adds dependency and migration overhead for a focused refactor.

2. Keep FastEndpoints as transport adapters only.
   - Decision: Endpoints validate/shape HTTP concerns and delegate business orchestration to handlers.
   - Rationale: Improves separation of concerns and makes logic easier to unit test.
   - Alternative considered: Keep mixed endpoint/service orchestration.
   - Why not chosen: Continues current inconsistency and testing friction.

3. Refactor operation-by-operation with parity checks.
   - Decision: Move create game, get game, and make move paths incrementally to handlers while preserving existing service collaborators.
   - Rationale: Reduces regression risk and keeps change reviewable.
   - Alternative considered: Big-bang rewrite.
   - Why not chosen: Higher integration risk and harder rollback.

4. Register handlers via DI and retain existing infrastructure wiring.
   - Decision: Add handler registrations in GameStateService composition root; keep MassTransit and repository registrations unchanged.
   - Rationale: Limits blast radius to request orchestration.
   - Alternative considered: Dynamic handler discovery.
   - Why not chosen: Extra complexity without clear value for current scope.

5. Colocate handler classes with FastEndpoint feature folders.
   - Decision: Place each request handler class in the same folder as its corresponding FastEndpoint class (for example, create-game handler beside create-game endpoint).
   - Rationale: Improves discoverability and keeps feature logic grouped for maintenance.
   - Alternative considered: Central `Handlers/` folder.
   - Why not chosen: Separates related endpoint/handler files and increases navigation overhead.

## Risks / Trade-offs

- Risk: Behavior drift during endpoint-to-handler extraction (status code or error mapping differences) -> Mitigation: Add/adjust endpoint-level tests for parity and validate representative scenarios.
- Risk: Extra abstraction may feel verbose for a small service -> Mitigation: Keep contracts minimal and colocate handlers with features.
- Risk: Incomplete migration leaves mixed patterns -> Mitigation: Refactor all current game-state operations in this change and document pattern for new endpoints.

## Migration Plan

1. Add request/handler contracts and DI registrations.
2. Implement handlers for create game, get game, and make move using existing collaborators.
3. Update endpoints to delegate to handlers.
4. Run unit/integration tests and compare API behavior with current expectations.
5. Deploy with standard rollout; monitor error rate and endpoint latency.

Rollback strategy:
- Revert GameStateService endpoint and wiring changes to previous endpoint-to-service flow.
- No data migration is involved, so rollback is code-only.

## Open Questions

- Should we formalize request/handler interfaces in a shared project for reuse by other backend services, or keep them local until a second adopter exists?
- Do we want a follow-up change to introduce pipeline behaviors (logging/validation) once handler adoption stabilizes?
