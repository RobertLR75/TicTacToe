## Context

The TicTacToe solution has a working FastEndpoints backend (`GameService`) with endpoints for creating games, retrieving game state, and making moves. The Blazor Server frontend (`TicTacToeMud`) uses MudBlazor v9 but currently only has template placeholder pages (Counter, Weather). No HttpClient is configured in the frontend.

Current backend API:
- `POST /api/games` — create a new game
- `GET /api/games/{id}` — get game state
- `POST /api/games/{id}/moves` — make a move (body: `{ gameId, row, col }`)

All three endpoints return the same shape: `{ gameId, currentPlayer, winner, isDraw, isOver, board: [{ row, col, mark }] }`

## Goals / Non-Goals

**Goals:**
- Provide a playable Tic-Tac-Toe UI using MudBlazor components that calls the backend API
- Use `SharedLibrary.HttpClient` NuGet package for typed HTTP client communication
- Remove unused template pages to keep the codebase clean
- Follow existing MudBlazor layout conventions

**Non-Goals:**
- Multiplayer/WebSocket real-time updates (single-browser, turn-based is sufficient)
- AI opponent — this is a two-player, same-device game
- Persistent game history or user accounts
- Backend API changes (existing endpoints are sufficient)
- Aspire integration (covered by separate change `add-tictactoemud-to-aspire`)

## Decisions

### 1. Use `SharedLibrary.HttpClient` for typed HttpClient

Add `SharedLibrary.HttpClient` as a NuGet package reference. Build a `GameApiClient` typed HttpClient service using this package. Register via `AddHttpClient<GameApiClient>()` in `Program.cs` with a configured base address.

**Rationale:** Follows the project's convention of using the shared HTTP client library. Provides consistent patterns for resilience, serialization, and DI registration across services.

### 2. MudBlazor grid layout for the game board

Use a 3x3 grid with `MudButton` cells. Each button displays X, O, or empty and is disabled when the game is over or the cell is occupied. Status text shows the current player, winner, or draw state.

**Why MudGrid over HTML table:** Consistent with MudBlazor styling, responsive, and leverages the existing MudBlazor theme (dark/light mode support out of the box).

### 3. Remove Counter and Weather pages, update navigation

Delete `Counter.razor` and `Weather.razor`. Update `NavMenu.razor` to replace their links with a single Game link. Update `Home.razor` to welcome users and link to the game.

**Why remove vs keep:** These are scaffolding pages with no value. Keeping them adds confusion for anyone exploring the codebase.

### 4. DTOs in the frontend project

Define response/request DTOs (`GameResponse`, `CellDto`) in the frontend project under a `Models/` folder. These mirror the backend response shapes.

**Why not share DTOs via a shared project:** The backend uses FastEndpoints request/response classes scoped to each endpoint namespace with duplicate `CellDto` definitions. Sharing would require refactoring the backend. Frontend DTOs are simpler and decoupled.

## Risks / Trade-offs

- **[Risk] `SharedLibrary.HttpClient` package not yet available locally** — The package needs to be published/available before the frontend can build. Mitigation: tasks are structured so the package reference is added first, making build failures immediately visible.
- **[Risk] Base address configuration** — The GameService URL needs to be configured (e.g., via `appsettings.json` or service discovery). Mitigation: use a configurable base address that can be overridden by Aspire service discovery once the `add-tictactoemud-to-aspire` change is applied.
- **[Trade-off] Duplicate DTOs** — Frontend DTOs duplicate backend response shapes. Acceptable for now; a shared contracts project could be introduced later if the API surface grows.
- **[Trade-off] No error handling UI beyond snackbar** — Initial implementation will show basic error states via `MudSnackbar` on API failure. Comprehensive retry/offline UX is out of scope.
