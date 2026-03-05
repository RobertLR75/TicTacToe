## Context

The backend project is currently named `GameService` (at `src/Backend/GameService/`), serving as the root namespace for all game state management: creation, retrieval, and move execution. The name `GameService` is generic and does not convey what aspect of the game this service is responsible for. As the system grows with additional backend services, a more descriptive name becomes important for navigability.

The project is referenced by:
- The solution file (`TicTacToe.sln`)
- The Aspire AppHost (`src/TicTacToe.AppHost/AppHost.cs` and its `.csproj`)
- 14 internal `.cs` files using `GameService` as the root namespace

There are no external consumers, no NuGet packages published from this project, and no database migrations tied to the project name. The Aspire resource name (`"gameservice"`) is an internal orchestration label.

## Goals / Non-Goals

**Goals:**
- Rename the project directory, `.csproj` file, and root namespace from `GameService` to `GameStateService`
- Update all references in the solution, AppHost, and source files
- Complete the rename as a single atomic commit for clean revert capability

**Non-Goals:**
- Changing API endpoints, request/response contracts, or any runtime behavior
- Renaming internal classes (`GameLogicService`, `GameRepository`) -- these already have descriptive names
- Introducing interface abstractions for existing services
- Updating OpenSpec documentation in prior/archived changes (they reflect historical context)

## Decisions

### 1. Rename scope: project + namespace only

**Decision**: Rename the directory, `.csproj` file, and all `namespace`/`using` declarations. Do not rename internal classes.

**Rationale**: The internal classes (`GameLogicService`, `GameRepository`) already have clear, specific names. Only the project-level name is ambiguous. Renaming classes would increase churn without improving clarity.

**Alternatives considered**:
- Rename everything including classes → Excessive churn, class names are already descriptive
- Only rename the directory/csproj without changing namespaces → Would create a mismatch between file path and namespace, violating C# conventions

### 2. Aspire resource name update

**Decision**: Update the Aspire resource name from `"gameservice"` to `"gamestateservice"` in `AppHost.cs`.

**Rationale**: The resource name should match the project name for consistency. This is an internal orchestration label with no external contract.

**Alternatives considered**:
- Keep old resource name `"gameservice"` → Would create confusion between project name and resource name
- Use a shortened name like `"gss"` → Less readable, saves nothing

### 3. Single atomic commit

**Decision**: Perform all renames in a single commit.

**Rationale**: Git can track directory renames when done atomically. This makes `git log --follow` work correctly and ensures a clean revert path.

## Risks / Trade-offs

- **[Merge conflicts for in-flight branches]** → Mitigation: Coordinate timing; perform rename when no large feature branches are pending merge. Since the rename touches namespaces in every file, any in-flight branch modifying `src/Backend/GameService/` files will conflict.
- **[CI/CD pipeline references]** → Mitigation: Verify Azure DevOps pipeline definitions do not hardcode `GameService` paths. If they reference the solution file, they will resolve automatically.
- **[IDE caches and local state]** → Mitigation: Team members should clean and rebuild after pulling (`dotnet clean && dotnet build`). Rider/VS may cache old project references.
