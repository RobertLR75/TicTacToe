## Context

The solution currently has two backend projects with inconsistent naming:
- `GameStateService` — handles in-game state, board logic, and move processing
- `GameApi` — handles game lobby lifecycle (create, list, activate, complete)

The `GameApi` name breaks the `*Service` naming convention established by `GameStateService` and the Aspire orchestration pattern. The project uses `GameApi.*` namespaces across 14 source files (28 namespace/using statements), and is referenced in the solution file, AppHost `.csproj`, and AppHost orchestration code.

No external consumers depend on the `GameApi` project name — the frontend only communicates with `GameStateService` via Aspire service discovery. The Aspire resource name `"gameapi"` is not referenced by any other service for discovery.

## Goals / Non-Goals

**Goals:**
- Rename the `GameApi` project to `GameService` across all layers: directory, `.csproj`, namespaces, solution file, and Aspire AppHost
- Maintain the existing `*Service` naming convention for backend projects
- Preserve all runtime behavior — endpoint paths, Redis key patterns, and API contracts remain unchanged

**Non-Goals:**
- Renaming `GameApiClient` in the frontend — this is a separate client class in `TicTacToeMud` that points at `GameStateService`, not `GameApi`. Its name is misleading but out of scope for this change.
- Changing any API endpoint paths (they stay at `/api/game-lobby/*`)
- Modifying any business logic or service behavior
- Updating archived change documentation in `openspec/changes/archive/`

## Decisions

### Decision 1: Target name `GameService`

**Choice**: Rename from `GameApi` to `GameService`.

**Rationale**: Follows the established `*Service` convention (`GameStateService`). The name accurately describes what the project is — a service handling game lobby operations.

**Alternatives considered**:
- `GameLobbyService` — More descriptive of the project's function but overly specific. If the project's scope expands beyond lobby management, the name would need to change again.
- `LobbyService` — Too generic and loses the `Game` domain context.

### Decision 2: Rename Aspire resource from `"gameapi"` to `"gameservice"`

**Choice**: Update both the Aspire resource name and HTTP endpoint name.

**Rationale**: The Aspire resource name should match the project name for consistency. Since no other services currently use `"gameapi"` for service discovery, this is safe.

### Decision 3: Preserve project GUID in solution file

**Choice**: Keep the existing project GUID `{28BD7577-ECA7-409E-8F51-8BEF5FC61E64}` and only update the project name and path.

**Rationale**: The GUID is used across build configuration sections in the `.sln` file. Changing it would require updating multiple additional lines with no benefit. Keeping it avoids unnecessary churn.

### Decision 4: Single atomic rename via filesystem move + find-and-replace

**Choice**: Rename the directory first, then do a bulk `GameApi` → `GameService` replacement across all source files in the renamed project, solution file, and AppHost.

**Rationale**: A bulk replacement is safe here because `GameApi` is used exclusively as a namespace root and project name — there are no partial matches or ambiguous usages that would cause false replacements.

## Risks / Trade-offs

- **[Risk] Build cache invalidation** → Mitigation: Clean `bin/` and `obj/` directories in the renamed project before rebuilding. Old assembly name artifacts could cause confusing build errors if not cleaned.
- **[Risk] IDE project reference caching** → Mitigation: Close and reopen the solution after the rename. Some IDEs cache project paths and may show stale errors.
- **[Risk] Confusion with `GameStateService`** → Mitigation: Both names are distinct (`GameService` vs `GameStateService`). The proposal scope is limited to the rename — no merging of the two projects.
