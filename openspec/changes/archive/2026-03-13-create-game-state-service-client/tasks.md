## 1. Client Setup

- [x] 1.1 Create `GameStateServiceClient` in `src/FrontEnd/TicTacToeMud/Services` with the existing game-state read and move methods moved from `GameApiClient`.
- [x] 1.2 Register `GameStateServiceClient` in `src/FrontEnd/TicTacToeMud/Program.cs` using the established GameStateService base-address pattern.
- [x] 1.3 Narrow `GameApiClient` so it only owns GameService-backed create/list operations.

## 2. Frontend Wiring

- [x] 2.1 Update gameplay components to inject and use `GameStateServiceClient` for initial state loading.
- [x] 2.2 Update gameplay components to use `GameStateServiceClient` for move submission while preserving existing notifications and hub-driven state refresh.

## 3. Verification

- [x] 3.1 Update or add frontend tests so create/list still stub `GameApiClient` and game-state flows stub `GameStateServiceClient`.
- [x] 3.2 Run the focused frontend test project and address any regressions caused by the client split.
