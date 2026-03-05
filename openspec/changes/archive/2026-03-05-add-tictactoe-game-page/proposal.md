## Why

The frontend currently has no game UI -- only placeholder Counter and Weather pages from the Blazor template. Users cannot play Tic-Tac-Toe despite a fully functional GameAPI backend. The frontend needs a game page that uses the backend API, and the unused template pages should be removed to keep the codebase clean.

## What Changes

- Add a new `/game` page with an interactive 3x3 Tic-Tac-Toe board using MudBlazor components
- Wire up the frontend to call GameAPI endpoints (`POST /api/games`, `GET /api/games/{id}`, `POST /api/games/{id}/moves`) via a typed HttpClient built on the `SharedLibrary.HttpClient` NuGet package
- **Remove** the Counter page (`/counter`) and Weather page (`/weather`)
- Update navigation to replace Counter/Weather links with a Game link
- Update the Home page to serve as a landing page directing users to the game

## Capabilities

### New Capabilities
- `game-page`: Interactive Tic-Tac-Toe game page with MudBlazor UI, real-time board state, move submission, win/draw detection display, and new game creation
- `game-api-client`: Typed HttpClient service for communicating with the GameAPI backend, built using the `SharedLibrary.HttpClient` NuGet package

### Modified Capabilities
<!-- None -- no existing spec-level behavior is changing -->

## Impact

- **Frontend (TicTacToeMud)**: New page added, two pages removed, navigation updated, `SharedLibrary.HttpClient` NuGet package added
- **Dependencies**: `SharedLibrary.HttpClient` local NuGet package reference added to `TicTacToeMud.csproj`
- **Backend (GameService)**: No backend changes
- **Rollback plan**: Revert the commit -- Counter/Weather pages can be restored from git history
- **Affected teams**: Frontend team
- **Performance impact**: Minimal -- adds HTTP calls from frontend to backend per game action; in-memory backend remains unchanged
