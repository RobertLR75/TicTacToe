## 1. Dependencies & HttpClient Setup

- [x] 1.1 ~~Add `SharedLibrary.HttpClient` NuGet package reference~~ — Skipped: package doesn't exist; using standard `AddHttpClient<T>()` patterns (resilience already provided by ServiceDefaults)
- [x] 1.2 Create `Models/GameResponse.cs` with DTOs: `GameResponse` record (`GameId`, `CurrentPlayer`, `Winner`, `IsDraw`, `IsOver`, `Board`) and `CellDto` record (`Row`, `Col`, `Mark`)
- [x] 1.3 Create `Services/GameApiClient.cs` typed HttpClient with methods: `CreateGameAsync()`, `GetGameAsync(gameId)`, `MakeMoveAsync(gameId, row, col)`
- [x] 1.4 Register `GameApiClient` in `Program.cs` via `AddHttpClient<GameApiClient>` with configurable base address

## 2. Game Page UI

- [x] 2.1 Create `Components/Pages/Game.razor` with `@page "/game"` route and `@rendermode InteractiveServer`
- [x] 2.2 Implement 3x3 MudBlazor grid with `MudButton` cells that display X, O, or empty
- [x] 2.3 Add game status display showing current player turn, winner, or draw
- [x] 2.4 Add "New Game" button that calls `CreateGameAsync()` and resets the board
- [x] 2.5 Wire cell click handlers to call `MakeMoveAsync()` and update board state
- [x] 2.6 Disable cell buttons when occupied or game is over
- [x] 2.7 Add error handling with `MudSnackbar` for API failures

## 3. Cleanup & Navigation

- [x] 3.1 Delete `Components/Pages/Counter.razor`
- [x] 3.2 Delete `Components/Pages/Weather.razor`
- [x] 3.3 Update `Components/Layout/NavMenu.razor` to remove Counter and Weather links, add Game link
- [x] 3.4 Update `Components/Pages/Home.razor` to serve as landing page with link to `/game`

## 4. Verification

- [x] 4.1 Build the solution and verify no compilation errors
