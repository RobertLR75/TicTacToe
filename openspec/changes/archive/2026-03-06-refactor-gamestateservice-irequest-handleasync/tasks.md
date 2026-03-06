## 1. Request Handler Foundations

- [x] 1.1 Add internal `IRequest<TResponse>` and `IRequestHandler<TRequest, TResponse>` contracts with `HandleAsync` in GameStateService.
- [x] 1.2 Register request handlers in DI while preserving existing infrastructure registrations.
- [x] 1.3 Define feature request/response models for create game, get game, and make move handler flows.

## 2. Handler Implementation

- [x] 2.1 Implement create-game `HandleAsync` handler in the same folder as the create-game FastEndpoint, using existing repository and event publishing dependencies.
- [x] 2.2 Implement get-game `HandleAsync` handler in the same folder as the get-game FastEndpoint, using existing repository read path and response mapping.
- [x] 2.3 Implement make-move `HandleAsync` handler in the same folder as the make-move FastEndpoint, using game logic, persistence, and event publishing collaborators.

## 3. Endpoint Refactor

- [x] 3.1 Update create-game endpoint to delegate orchestration to the request handler and keep `202 Accepted` response shape unchanged.
- [x] 3.2 Update get-game endpoint to delegate orchestration to the request handler without contract changes.
- [x] 3.3 Update make-move endpoint to delegate orchestration to the request handler without contract changes.

## 4. Verification

- [x] 4.1 Add or update unit tests for each handler `HandleAsync` path (success and key failure cases).
- [x] 4.2 Add or update endpoint-level tests to verify API response/status parity after refactor.
- [x] 4.3 Run solution build/tests and confirm no regressions in GameStateService behavior.
