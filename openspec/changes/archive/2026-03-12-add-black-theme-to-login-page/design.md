## Context

The login page already uses MudBlazor-themed structure, but the requested experience is an explicit black theme presentation for the login route. This change should remain frontend-only and preserve existing login/session behavior while improving visual consistency with dark-brand styling.

Current constraints:
- UI stack is Blazor + MudBlazor.
- Existing login flow, session creation, and routing should stay unchanged.
- Accessibility and responsive behavior from the current login-page-theming capability must be retained.

Stakeholders:
- Frontend team (implementation)
- QA team (rendering/UX verification)
- Product/design stakeholders (theme acceptance)

## Goals / Non-Goals

**Goals:**
- Apply an explicit black theme presentation to the login page.
- Keep text, controls, and button states clearly readable with accessible contrast.
- Preserve current login form behavior, validation, and navigation/session outcomes.
- Maintain mobile and desktop usability with no layout regression.

**Non-Goals:**
- Reworking global application theming beyond login route scope.
- Changing authentication/session contracts or backend APIs.
- Redesigning login information architecture or adding new login fields.

## Decisions

1. **Use login-page scoped MudBlazor styling updates instead of global theme overhaul**
   - **Decision:** Implement black-theme treatment at the login page component/style scope.
   - **Rationale:** Minimizes regression risk outside login and keeps change focused.
   - **Alternative considered:** Update the global MudTheme palette to black defaults.
   - **Why not alternative:** Could unintentionally alter all pages and exceeds requested scope.

2. **Keep existing login component structure and behavior intact**
   - **Decision:** Preserve route, form controls, validation flow, and submit behavior while only adjusting visual treatment.
   - **Rationale:** Avoids behavior regressions and keeps backwards compatibility.
   - **Alternative considered:** Refactor login component structure while applying theme.
   - **Why not alternative:** Adds unnecessary risk for a style-focused change.

3. **Verify with focused frontend tests for themed rendering and behavior parity**
   - **Decision:** Add/update tests to assert expected themed markers and unchanged validation/submission behavior.
   - **Rationale:** Prevents future style or behavior regressions.
   - **Alternative considered:** Manual visual verification only.
   - **Why not alternative:** Insufficient repeatable coverage.

## Risks / Trade-offs

- **[Risk] Black background may reduce contrast for existing text/button colors** → **Mitigation:** explicitly validate readable color contrast and focus states in login component.
- **[Risk] Scoped style overrides may conflict with MudBlazor defaults after package updates** → **Mitigation:** keep styles minimal and use MudBlazor utility/palette patterns where possible.
- **[Trade-off] Page-scoped theming may duplicate small style intent versus global theme tokens** → **Mitigation:** accept minor duplication to keep blast radius small.

## Migration Plan

1. Update login page visual styling to apply black-themed presentation.
2. Verify responsive layout/focus/validation states remain usable.
3. Add/update frontend tests for login themed rendering and behavior parity.
4. Validate no changes to login session flow and navigation outcomes.

Rollback:
- Revert login-page scoped styling/test updates to restore prior login visuals.
- No backend rollback required.

## Open Questions

- Should the black theme be pure black (`#000`) or near-black to improve readability for long sessions?
- Should brand accent colors for primary actions be adjusted as part of this change, or kept as-is?
