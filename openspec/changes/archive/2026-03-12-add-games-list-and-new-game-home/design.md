## Context

The frontend already supports game listing and game creation through `GameApiClient`, but the current entry experience requires users to navigate to the `/game` page before performing those actions. This change introduces a lightweight home-page game hub while preserving existing API contracts and existing `/game` behavior.

Constraints and context:
- Frontend uses Blazor + MudBlazor patterns with typed HttpClient services.
- Existing backend endpoints (`GET /api/games`, `POST /api/games`) remain unchanged.
- Existing game page behavior and SignalR update flows must continue to work.
- The change should remain backward compatible and low risk.

Stakeholders:
- Frontend team (implementation)
- QA team (UI and behavior validation)
- Product stakeholders focused on reducing friction for starting gameplay

## Goals / Non-Goals

**Goals:**
- Add a home-page UI section that lists existing games from `GameApiClient.ListGamesAsync()`.
- Add a home-page "New Game" action that calls `GameApiClient.CreateGameAsync(...)`.
- Refresh the list after successful creation so users immediately see the newly created game.
- Keep user feedback consistent with existing notification patterns.
- Preserve existing `/game` page behavior and API compatibility.

**Non-Goals:**
- Changing backend API contracts, payloads, or endpoint routes.
- Replacing SignalR game-state behavior.
- Introducing new global state architecture or additional UI frameworks.
- Redesigning the full navigation/layout system.

## Decisions

1. **Use existing typed API client (`GameApiClient`) for home-page data/actions**
   - **Decision:** Home page calls existing typed methods for list/create.
   - **Rationale:** Avoids duplicate HTTP logic and keeps API integration centralized and testable.
   - **Alternative considered:** Direct `HttpClient` calls from page component.
   - **Why not alternative:** Violates frontend architecture constraints and increases coupling.

2. **Keep home-page behavior independent from `/game` real-time board flow**
   - **Decision:** Home page focuses on list/create only; game board and SignalR interactions remain in `/game`.
   - **Rationale:** Keeps concerns separated and minimizes regression risk in existing board interaction.
   - **Alternative considered:** Move all game interactions to home page.
   - **Why not alternative:** Larger scope, higher risk, and unnecessary for requested change.

3. **Refresh list after successful create instead of optimistic insertion**
   - **Decision:** Re-fetch games after create completes.
   - **Rationale:** Guarantees consistency with backend source of truth and avoids duplicate mapping edge cases.
   - **Alternative considered:** Optimistically append newly created game.
   - **Why not alternative:** Requires additional assumptions about shape/timing and can drift from server state.

4. **Use existing notification mechanism for success/failure feedback**
   - **Decision:** Surface list/create outcomes through existing snackbar/notification conventions.
   - **Rationale:** Consistent UX and centralized message behavior.
   - **Alternative considered:** Inline ad-hoc text messages only.
   - **Why not alternative:** Inconsistent with existing app feedback patterns.

## Risks / Trade-offs

- **[Risk] Duplicate create/list controls between home and game pages may create minor UX redundancy** → **Mitigation:** Keep copy and behavior consistent; align labels and action outcomes across pages.
- **[Risk] Additional home-page list fetch increases startup requests** → **Mitigation:** Single lightweight call on load; no polling.
- **[Risk] Failed create/list calls could confuse users** → **Mitigation:** Clear error notifications and preserve previous UI state.
- **[Trade-off] Re-fetching after create adds one extra request** → **Mitigation:** Accept small overhead for correctness and simpler implementation.

## Migration Plan

1. Implement home-page UI additions (game list + new game button) behind existing route.
2. Wire actions to `GameApiClient` list/create methods.
3. Add post-create refresh behavior and user notifications.
4. Add/update bUnit tests for home-page rendering, create action, and refresh flow.
5. Validate existing `/game` page behaviors remain unchanged.

Rollback:
- Revert home-page game list/create component changes.
- Remove any added wiring for home-page calls.
- No backend rollback required since API contracts are unchanged.

## Open Questions

- Should selecting a listed game on the home page immediately navigate to `/game` with that game preselected, or remain list-only for this change?
- Should the home-page list show minimal columns (game id/status) or match the full game page list detail level?
