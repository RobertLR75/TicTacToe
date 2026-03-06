## Context

The application currently allows users to land on the homepage without a session-backed identity. Gameplay and UI flows implicitly assume a user identity exists, which can lead to null/anonymous behavior and inconsistent state handling.

This change introduces a lightweight login experience only for session bootstrap. The solution must preserve existing homepage behavior once session exists, avoid external identity providers, and remain backward compatible with current public APIs.

## Goals / Non-Goals

**Goals:**
- Enforce that homepage access requires a session user containing `UserId` (GUID) and `Name` (string).
- Provide a minimal login form that collects username and creates session identity.
- Redirect users back to homepage after successful session creation.
- Keep implementation small and testable with unit/integration coverage of redirect and session behavior.

**Non-Goals:**
- Full authentication/authorization (passwords, OAuth, roles, claims).
- Persistent user account management in a database.
- Changes to game logic beyond requiring a valid session identity.

## Decisions

### Decision: Add a route-level session guard for homepage entry
- Choice: Validate session user in the homepage entry handler (or equivalent route middleware) and redirect to login when absent.
- Rationale: Keeps access rule centralized and ensures all direct homepage loads are protected.
- Alternatives considered:
  - Client-only check after page load: rejected because users can briefly access protected view and logic.
  - Guard every component independently: rejected due to duplication and drift risk.

### Decision: Implement a simple login page that only collects username
- Choice: Render a basic form with one required `Name` field.
- Rationale: Meets the requirement for session bootstrap with minimal UX and implementation complexity.
- Alternatives considered:
  - Auto-generate anonymous names: rejected because the requirement asks users to provide username.
  - Multi-field profile form: rejected as out-of-scope for session gate.

### Decision: Generate `UserId` server-side and store `{ UserId, Name }` in session
- Choice: On successful form submit, create GUID server-side and persist both values in session storage.
- Rationale: Prevents client tampering of identity keys and keeps session schema explicit.
- Alternatives considered:
  - Client-generated GUID: rejected for trust and consistency reasons.
  - Name-only session object: rejected because requirement explicitly needs `UserId` and `Name`.

### Decision: Preserve return-to-home redirect flow
- Choice: If no session exists, redirect homepage requests to login; on successful login, redirect to home.
- Rationale: Predictable user flow and minimal navigation complexity.
- Alternatives considered:
  - Modal login overlay on homepage: rejected because page should be protected before rendering.

## Risks / Trade-offs

- [Risk] Session key name mismatches across handlers can break guard logic silently. -> Mitigation: Define a single constant for session key and reuse it.
- [Risk] Empty/whitespace usernames create invalid session records. -> Mitigation: Validate and reject invalid input with inline error.
- [Risk] Redirect loops if login route is incorrectly guarded. -> Mitigation: Exclude login route from homepage session guard.
- [Trade-off] Users are identified only per-session, not persistently across browsers/devices. -> Mitigation: Accept as intentional scope; can be extended later.

## Migration Plan

1. Add session user contract and shared session-key constant.
2. Implement homepage session guard and redirect behavior.
3. Add login page GET/POST handlers and validation.
4. On successful login, create session payload and redirect to homepage.
5. Add/adjust automated tests for guard, login validation, and redirect flow.
6. Deploy with standard release process.

Rollback strategy:
- Revert the homepage guard and login route wiring.
- Remove new session payload usage if needed.
- Redeploy previous stable build.

## Open Questions

- Should username length/character constraints be enforced beyond non-empty (for example max length 50)?
- Should an existing session user be allowed to revisit `/login`, or should they always be redirected to homepage?
