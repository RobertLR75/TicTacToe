## Why

The frontend currently starts a single new game flow without presenting previously created games, which makes it hard for users to discover and resume active games. We need a simple game list experience that can create games with user identity and immediately refresh visible data.

## What Changes

- Add a GameList UI that fetches created games from `GameService.List` (`GET /api/games`) and renders them to the user.
- Add a "New Game" action in the list UI that calls `GameService.Create` (`POST /api/games`) with `playerId` and `playerName`.
- Refresh the displayed game list after successful game creation so the newly created game appears without manual reload.
- Preserve existing backend API compatibility and response semantics.

## Capabilities

### New Capabilities
- None.

### Modified Capabilities
- `game-page`: Extend page behavior to include loading and displaying a list of games plus a create-and-refresh interaction.
- `frontend-api-client`: Extend typed client with list-games support and create-game request payload fields for `playerId` and `playerName`.

## Impact

- Affected code: frontend page/components for game discovery and the typed API client request/response contracts.
- Affected APIs: frontend usage of `GET /api/games` and `POST /api/games`; no server endpoint shape change is proposed.
- Performance impact: one additional list fetch on initial load and one list refresh after successful create; should remain low-volume and bounded by current paging semantics.
- Affected teams: frontend/web team (primary), backend API team (awareness only to validate contract usage), QA (regression and UX validation).
- Rollback plan: revert frontend GameList UI and client contract changes to restore previous single-game flow; backend remains unchanged so rollback is low risk.
