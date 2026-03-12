## 1. Home Page UI and Interaction

- [x] 1.1 Identify the home page component(s) and add a MudBlazor game list section with loading and empty states.
- [x] 1.2 Add a MudBlazor "New Game" button on the home page and wire the click handler for create flow.
- [x] 1.3 Ensure the home page remains responsive and interactive during API operations (loading indicators, disabled action while in-flight).

## 2. API and State Wiring

- [x] 2.1 Integrate home page initialization with `GameApiClient.ListGamesAsync()` and map results into UI-friendly view models.
- [x] 2.2 Integrate the home page new-game action with `GameApiClient.CreateGameAsync(playerId, playerName)` using current user identity context.
- [x] 2.3 Refresh the home-page game list via `ListGamesAsync()` after successful create so the new game appears immediately.

## 3. Notifications and Error Handling

- [x] 3.1 Add success/error notification behavior for list and create outcomes using the existing frontend notification pattern.
- [x] 3.2 Verify API failures do not break the page and keep existing game list state stable when create fails.

## 4. Validation and Tests

- [x] 4.1 Add or update bUnit tests for home-page initial list load, empty state rendering, and list rendering behavior.
- [x] 4.2 Add or update bUnit tests for home-page new-game flow, including post-create refresh call verification.
- [x] 4.3 Add or update bUnit tests for list/create error notifications and non-destructive failure behavior.

## 5. Regression and Readiness

- [x] 5.1 Verify existing `/game` page new-game behavior remains unchanged and still refreshes its game list.
- [x] 5.2 Run relevant frontend test suite(s) and fix any regressions introduced by the home-page changes.
