## Why

The login page currently works functionally but does not align with the MudBlazor visual language used elsewhere in the frontend. Applying a consistent MudBlazor theme to the login experience improves brand consistency, usability, and first-impression quality for users entering the app.

## What Changes

- Apply a MudBlazor-driven themed layout and styling to the login page while preserving existing login flow and validation behavior.
- Introduce or update theme tokens (palette/typography/spacing) used by the login view so the page matches the established application design language.
- Ensure the updated login page remains responsive across desktop and mobile breakpoints.
- Validate accessibility fundamentals (contrast, focus visibility, keyboard navigation) after theming updates.
- Add targeted frontend tests for rendering and key themed UI states where existing test coverage is insufficient.
- Define a rollback path by keeping changes scoped to login page/theme configuration so the previous login appearance can be restored quickly by reverting this change set.

## Capabilities

### New Capabilities
- `login-page-theming`: Defines visual and UX requirements for a MudBlazor-themed login page, including responsive layout and accessible styling.

### Modified Capabilities
- `frontend-session-login-gate`: Clarifies that login gating behavior is preserved while the login route presents the themed MudBlazor experience.

## Impact

- Affected code: Frontend login page components, shared MudBlazor theme configuration, and related CSS/style resources in `src/FrontEnd/TicTacToeMud`.
- Affected APIs: None expected; no backend contract changes.
- Dependencies/systems: MudBlazor theme setup and frontend test project (`tests/TicTacToeMud.Tests`).
- Affected teams: Frontend/UI team (implementation), QA (visual/accessibility validation), and Product/Design stakeholders (theme acceptance).
- Performance impact analysis: Low expected runtime impact; theming changes are primarily static style/render updates. Verify no noticeable increase in initial login page render time and no layout shift regressions on common viewport sizes.
