## 1. Frontend API Client Updates

- [ ] 1.1 Update `GameApiClient` create-game method signature to require `playerId` and `playerName` and send both fields in `POST /api/games`.
- [ ] 1.2 Add a typed list-games client method that calls `GET /api/games` and maps the response model used by the UI.
- [ ] 1.3 Update or add unit tests for `GameApiClient` to verify request payloads, routes, and expected response handling.

## 2. Game Page UI and Interaction Flow

- [ ] 2.1 Add GameList loading on `/game` page initialization using the list-games client method.
- [ ] 2.2 Implement GameList rendering for created games using existing frontend component patterns.
- [ ] 2.3 Add a "New Game" button that calls create-game with available `playerId` and `playerName` values.
- [ ] 2.4 Keep current board/SignalR start flow for newly created games using returned `gameId` and `GameCreated` notification handling.
- [ ] 2.5 Refresh GameList after successful game creation and surface user-visible error feedback when list/create calls fail.

## 3. Verification and Regression Coverage

- [ ] 3.1 Add or update UI/component tests to cover initial list load, new game creation, and post-create list refresh.
- [ ] 3.2 Run existing test suites relevant to frontend page behavior and API client integration; fix any regressions.
- [ ] 3.3 Perform manual validation of `/game` flow: initial list display, create with identity fields, and refreshed list containing the new game.
