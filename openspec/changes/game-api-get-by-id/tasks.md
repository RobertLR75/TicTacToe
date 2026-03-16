## 1. Service integration setup

- [x] 1.1 Add a GameService feature slice for `GET /api/games/{gameId}` with endpoint, request orchestration, and dependency registration.
- [x] 1.2 Introduce the GameStateService read gateway/typed client abstraction that GameService will use to fetch current game state.

## 2. Public API implementation

- [x] 2.1 Implement the GameService get-by-id handler to translate successful, missing, and unavailable state responses into the public API contract.
- [x] 2.2 Ensure the route returns the shared `GetGameResponse` payload shape expected by frontend consumers without changing existing create, list, or move contracts.

## 3. Verification

- [x] 3.1 Add unit tests for GameService get-by-id orchestration, including success, not-found, and dependency-unavailable cases.
- [x] 3.2 Add or update integration tests covering `GET /api/games/{gameId}` end-to-end behavior and confirm frontend/client contract alignment.
