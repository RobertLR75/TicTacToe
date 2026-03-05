## 1. Backend - NotificationService

- [x] 1.1 Update `NotifyGameCreated` in `NotificationService.cs` to send full game state (same shape as `GameUpdateNotification`) instead of just `{ GameId }`
- [x] 1.2 Create `GameCreatedNotification` record with fields: `GameId`, `CurrentPlayer`, `Winner`, `IsDraw`, `IsOver`, `Board` (matching `GameUpdateNotification` shape)

## 2. Backend - CreateGameEndpoint

- [x] 2.1 Inject `NotificationService` into `CreateGameEndpoint` constructor
- [x] 2.2 Update `CreateGameResponse` to contain only `GameId` (remove `CurrentPlayer`, `Winner`, `IsDraw`, `IsOver`, `Board`)
- [x] 2.3 Change `HandleAsync` to call `NotificationService.NotifyGameCreated(game)` after creating the game
- [x] 2.4 Change `HandleAsync` to return `202 Accepted` with `{ GameId }` instead of `200 OK` with full state

## 3. Backend - MakeMoveEndpoint

- [x] 3.1 Change `MakeMoveEndpoint` to return `202 Accepted` with no response body instead of `200 OK` with game state
- [x] 3.2 Remove or simplify `MakeMoveResponse` (endpoint no longer returns it in the body)

## 4. Frontend - GameApiClient

- [x] 4.1 Update `CreateGameAsync` to return `string` (gameId) instead of `GameResponse`, parsing only `{ gameId }` from the `202` response
- [x] 4.2 Update `MakeMoveAsync` to return `Task` (void) instead of `Task<GameResponse>`, only ensuring `202` success status

## 5. Frontend - GameHubClient

- [x] 5.1 Update `GameCreatedNotification` record to include full game state fields (`CurrentPlayer`, `Winner`, `IsDraw`, `IsOver`, `Board`) matching `GameUpdateNotification`
- [x] 5.2 Update `OnGameCreated` event signature from `Action<string>` to `Action<GameResponse>` and map the notification to `GameResponse`

## 6. Frontend - Game.razor

- [x] 6.1 Add `HandleGameCreated` handler that receives `GameResponse` from `GameCreated` notification, filters by `gameId`, and calls `UpdateState`
- [x] 6.2 Subscribe to `GameHub.OnGameCreated` in `ConnectToHub` and unsubscribe in `DisposeAsync`
- [x] 6.3 Update `StartNewGame` to use `string gameId` from `CreateGameAsync` return value, join SignalR group, and let `GameCreated` notification populate state
- [x] 6.4 Update `OnCellClick` to call `MakeMoveAsync` without reading a response — state arrives via existing `HandleGameUpdated` handler
- [x] 6.5 Clear board state after calling `CreateGameAsync` and before `GameCreated` notification arrives (show loading state)

## 7. Verification

- [x] 7.1 Build solution and fix any compilation errors
- [x] 7.2 Manually verify: creating a game returns `202` with `{ gameId }` and board populates via SignalR
- [x] 7.3 Manually verify: making a move returns `202` with no body and board updates via SignalR
