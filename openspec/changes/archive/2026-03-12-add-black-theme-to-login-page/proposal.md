## Why

The login page should align with the requested dark visual style so first-time user experience is consistent with product branding and user preference for a black-themed entry screen. Implementing this now keeps UI consistency as more MudBlazor theming work is being added.

## What Changes

- Update login page styling to use a black-themed presentation (background, container, and text/action contrast) while preserving existing login behavior.
- Ensure the themed login page remains responsive and accessible across mobile and desktop breakpoints.
- Keep current login route, form validation behavior, and session flow unchanged.
- Add or update frontend tests to verify themed rendering and login usability under the new black style.

## Capabilities

### New Capabilities
- *(none)*

### Modified Capabilities
- `login-page-theming`: refine requirement expectations so the login page uses an explicit black theme variant with accessible contrast and unchanged login workflow behavior.

## Impact

- **Affected code**: `src/FrontEnd/TicTacToeMud` login page component(s), MudBlazor theme usage/styling, and `tests/TicTacToeMud.Tests` login UI tests.
- **Affected APIs**: none.
- **Affected teams**: Frontend team (primary), QA team (UI validation), Design/Product (theme acceptance).
- **Performance impact**: negligible; visual-only styling adjustments with no expected API or heavy render overhead.
- **Rollback plan**: revert login-page theme changes and restore previous login styling while keeping existing login behavior.
