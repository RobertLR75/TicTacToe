## Context

The frontend currently emphasizes a single active game flow and does not present a discoverable list of existing games. Backend endpoints for listing and creating games already exist, but the UI and typed client do not fully support a list-first workflow with explicit creator identity fields. The change should remain backward compatible at the public API layer and avoid backend schema or contract changes.

## Goals / Non-Goals

**Goals:**
- Render a GameList experience populated from `GET /api/games`.
- Support creating a new game from the same UI by calling `POST /api/games` with `playerId` and `playerName`.
- Refresh the list after a successful create so users immediately see the new game.
- Keep implementation localized to frontend page/component and typed API client updates.

**Non-Goals:**
- Changing backend endpoint routes, response codes, or persistence behavior.
- Adding advanced filtering, sorting, or pagination controls beyond current endpoint defaults.
- Implementing auth/identity management; this change only forwards available user identity values.

## Decisions

- Build the list workflow on top of the existing typed `GameApiClient` by adding/using list and create request methods.
  - Rationale: keeps HTTP contract logic in one place and preserves existing frontend architecture.
  - Alternative considered: invoke `HttpClient` directly in page component. Rejected to avoid duplicated API logic and weaker testability.
- Model create payload as explicit `playerId` and `playerName` in the frontend API client call.
  - Rationale: aligns with `create-game` API requirement and prevents accidental anonymous creates.
  - Alternative considered: keep parameterless create call and inject defaults server-side. Rejected because it obscures caller intent and violates current API contract.
- Refresh list with a full re-fetch after successful create instead of optimistic local append.
  - Rationale: avoids client-side drift and keeps ordering/filter semantics consistent with backend list behavior.
  - Alternative considered: append returned data locally. Rejected because create response only returns `gameId` and may not include all list fields.
- Preserve existing game play flow while adding list/create entry point.
  - Rationale: minimize regression risk in current `/game` interactions.
  - Alternative considered: replace existing page behavior entirely. Rejected due to higher migration and testing risk.

## Risks / Trade-offs

- [Risk] Additional list fetches can increase frontend request volume. → Mitigation: call refresh only on initial load and after successful create.
- [Risk] Missing/invalid user identity values can block create calls. → Mitigation: validate identity presence before request and show user-facing error states.
- [Risk] UI complexity increases by combining list and create interactions on one page. → Mitigation: keep component responsibilities separated (list rendering vs create action).
- [Trade-off] Full re-fetch favors correctness over minimal network usage. → Mitigation: rely on existing lightweight list endpoint and revisit incremental updates only if needed.

## Migration Plan

1. Extend frontend API client contracts for list and create payload usage.
2. Update game page/component to load game list and render it.
3. Add New Game action wired to create endpoint with identity fields.
4. Trigger list refresh after successful creation and maintain existing error handling patterns.
5. Validate via frontend/unit tests and manual UI flow checks.

Rollback strategy:
- Revert frontend component/client changes to return to previous game page behavior. Backend services require no rollback.

## Open Questions

- Which concrete source in the frontend should provide `playerId` and `playerName` when no authentication session is present?
- Should the list UI include direct navigation/resume actions now, or remain display-and-refresh only for this change?
