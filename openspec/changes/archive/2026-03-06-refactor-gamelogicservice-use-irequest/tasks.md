## 1. Game Logic Request Contracts

- [x] 1.1 Define `IRequest<TResponse>` request/response models for game-logic operations currently invoked via `GameLogicService`.
- [x] 1.2 Add handler interfaces/implementations for game-logic operations using `HandleAsync`.
- [x] 1.3 Place request and handler classes in the same feature area as `GameLogicService` for discoverability.

## 2. Integration and Refactor

- [x] 2.1 Register game-logic request handlers in dependency injection without changing external infrastructure wiring.
- [x] 2.2 Refactor existing orchestration call sites to use injected handlers instead of direct `GameLogicService` method calls.
- [x] 2.3 Preserve event publication sequencing and move outcome mapping during the refactor.

## 3. Verification

- [x] 3.1 Add or update unit tests for new game-logic handlers covering success and key failure paths.
- [x] 3.2 Add or update parity tests to confirm gameplay outcomes remain equivalent after refactor.
- [x] 3.3 Run GameStateService build/tests and confirm no regressions in game-state update behavior.
