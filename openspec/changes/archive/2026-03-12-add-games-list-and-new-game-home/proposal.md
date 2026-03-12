## Why

The home page currently does not provide a quick way to see existing games or start a new one, which adds unnecessary navigation friction for core gameplay workflows. We should expose these actions directly on the landing experience to improve usability and reduce time-to-play.

## What Changes

- Add a home page game list that shows existing games loaded from `GameApiClient.ListGamesAsync()`.
- Add a "New Game" button on the home page that creates a game via `GameApiClient.CreateGameAsync(...)`.
- Refresh the home page game list after successful game creation so the newly created game appears immediately.
- Show user-friendly success and failure feedback for list/create operations using the existing frontend notification pattern.
- Keep backend API contracts unchanged (no breaking API changes).

## Capabilities

### New Capabilities
- `home-page-games`: Home page experience for listing games and creating a new game using typed frontend API client calls.

### Modified Capabilities
- `game-page`: Clarify that game listing and new-game creation actions are also available from the home page entry experience, while preserving existing game page behavior.

## Impact

- **Affected code**: `src/FrontEnd/TicTacToeMud` home page components/pages, related view models/state, and bUnit tests in `tests/TicTacToeMud.Tests`.
- **Affected APIs**: No backend endpoint changes; frontend will use existing `GET /api/games` and `POST /api/games` contracts.
- **Affected teams**: Frontend/UI team (primary), QA/testing team (test coverage updates), backend team (awareness only; no contract change expected).
- **Performance impact**: One additional home page API fetch on load and one post-create refresh fetch; expected low impact due to small payload and existing endpoint behavior.
- **Rollback plan**: Revert home page integration and hide/remove the new list/button UI, restoring previous home page behavior without requiring backend rollback.
