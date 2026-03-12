## Why

Authenticated users can enter the app through the session-based login flow, but they currently have no visible way to end that session without manually clearing browser state. Adding a logout control in the top navigation gives users an expected exit path on every page and makes switching players simpler during local gameplay.

## What Changes

- Add a persistent logout action to the top navigation bar so it is available across the frontend experience.
- Clear the current session user when logout is triggered and redirect the user back to the login page.
- Preserve existing login gating so pages that require a session continue to redirect unauthenticated users.

## Capabilities

### New Capabilities
- `frontend-session-logout`: Defines the top navigation logout experience, session clearing behavior, and redirect flow after sign-out.

### Modified Capabilities

## Impact

- Affected code: `src/FrontEnd/TicTacToeMud/Components/Layout/MainLayout.razor`, `src/FrontEnd/TicTacToeMud/Components/Layout/NavMenu.razor`, and supporting frontend session services/components used by the login flow.
- Affected systems: Blazor frontend layout and session-based authentication flow.
- Affected teams: Frontend/UI maintainers and anyone validating session/login behavior.
- Performance impact: Negligible; the change adds one UI action and a session clear/redirect path with no meaningful runtime cost.
- Rollback plan: Remove the logout UI and restore the current layout/session flow if the new sign-out behavior causes regressions.
