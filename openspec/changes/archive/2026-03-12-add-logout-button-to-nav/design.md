## Context

The frontend currently creates and reads a session-backed `SessionUser` during the `/login` flow, and the root route redirects unauthenticated users back to login. Once a session is established, the shared application shell renders a top app bar and side navigation for all interactive pages, but there is no first-class sign-out action in that shell. This change adds a logout control without changing the existing session identity format or login endpoint behavior.

Stakeholders include frontend maintainers implementing the UI, QA validating session transitions, and developers/demo users who need an obvious way to switch players during gameplay.

## Goals / Non-Goals

**Goals:**
- Expose a logout action from the shared top navigation so it is available on all authenticated pages.
- Clear the current session user and return the browser to `/login` when logout is invoked.
- Reuse the existing session/login gate rather than introducing a second authentication model.
- Keep the solution small and aligned with current Blazor and MudBlazor patterns.

**Non-Goals:**
- Changing how users log in or how `SessionUser` is created.
- Introducing identity providers, claims-based auth, or backend token handling.
- Redesigning the rest of the application shell beyond what is needed to place the logout action.

## Decisions

### Decision: Implement logout as a dedicated frontend endpoint
- **Choice:** Add a `/logout` endpoint in `Program.cs` that clears the session user and redirects to `/login`, and have the top navigation trigger that endpoint.
- **Rationale:** The current login flow is already defined with minimal endpoints and session helpers. A dedicated logout endpoint keeps session mutation in one server-side place and avoids duplicating session-clearing logic in multiple components.
- **Alternatives considered:**
  - Clear session directly inside a Razor component: rejected because the current app already centralizes session creation in server endpoints and `HttpContext` access inside components is more fragile.
  - Add a JavaScript-driven sign-out flow: rejected because it adds unnecessary client-side complexity.

### Decision: Place the logout action in the shared app bar
- **Choice:** Render the logout control in `MainLayout.razor` so it appears at the top of every page using the main layout.
- **Rationale:** The request is specifically for a top navigation action on all pages, and the app bar is the shared top-level surface already present across the authenticated experience.
- **Alternatives considered:**
  - Put logout in `NavMenu.razor`: rejected because that is sidebar navigation, not the top bar requested by the user.
  - Add per-page logout buttons: rejected because it would duplicate UI and could create inconsistent placement.

### Decision: Extend the session store with explicit clear semantics
- **Choice:** Add a `Clear` helper to `SessionUserStore` and use it from the logout endpoint.
- **Rationale:** The store already owns session persistence details; adding a matching clear operation keeps session key handling encapsulated and easy to test.
- **Alternatives considered:**
  - Call `session.Remove(...)` inline in `Program.cs`: rejected because it leaks storage details outside the session abstraction.

## Risks / Trade-offs

- [Logout appears on routes that do not require sign-in] -> Mitigation: Scope rendering to the shared authenticated layout and avoid adding the control to the standalone login route.
- [Session is cleared but the current interactive UI does not immediately reflect it] -> Mitigation: Use a full redirect to `/login` after logout so the app reloads in an unauthenticated state.
- [Future auth changes outgrow the minimal endpoint approach] -> Mitigation: Keep the logout action isolated behind `SessionUserStore` and route handlers so it can be replaced later.

## Migration Plan

1. Add session-clearing support to `SessionUserStore`.
2. Add a logout endpoint that clears the session and redirects to `/login`.
3. Add a logout button/link to the shared top app bar in `MainLayout.razor`.
4. Add or update frontend tests to verify logout visibility and post-logout redirect behavior.
5. Validate the flow manually by logging in, navigating to a page, and logging out.

Rollback strategy:
- Remove the logout endpoint, session clear helper, and app bar control to restore the current behavior.
- No data migration or backend contract rollback is required.

## Open Questions

- None.
