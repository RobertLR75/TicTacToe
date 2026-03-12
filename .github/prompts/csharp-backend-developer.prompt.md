---
description: Use the C# backend developer skill for backend implementation tasks
---

Use the `csharp-backend-developer` skill to implement or refactor backend code in this repository.

## Input

Optional scope after the prompt, for example:
- `/csharp-backend-developer GameService update-status endpoint`
- `/csharp-backend-developer add unit tests for GameStateService handlers`

## Behavior

1. Load and follow the `csharp-backend-developer` skill.
2. Keep changes minimal and scoped to the requested backend area.
3. Preserve API compatibility unless the request explicitly allows breaking changes.
4. Add/update tests for behavior changes.
5. Run targeted tests first, then broader tests when appropriate.

## Output

- Briefly describe what changed and why.
- List files touched.
- Report test commands executed and results.
- Call out blockers or follow-up work if anything could not be completed.
