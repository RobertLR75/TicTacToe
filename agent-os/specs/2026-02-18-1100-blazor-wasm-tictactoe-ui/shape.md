# Shape: Blazor WebAssembly TicTacToe UI

## Scope

- Local 2-player TicTacToe (two players on same screen, taking turns)
- No backend required; all logic runs in the browser via Blazor WASM
- Greenfield frontend; backend GameService class library remains unchanged

## Key Decisions

- **Blazor WASM** chosen for .NET/C# consistency with backend standards
- **GameService as scoped DI service** (not static) for future multi-game support
- **Records for value objects** (`Cell`, `Move`), **classes for mutable entities** (`Board`, `GameState`) — per csharp-type-design standard
- **File-scoped namespaces, one type per file** — per csharp-file-structure standard
- **Each component in its own folder** with co-located files — adapted from razor-pages/folder-structure
- **Route "/" assigned to game page** for immediate access on load
- **Fully qualified model types in Razor code blocks** to avoid name conflicts between model records (`TicTacToeUI.Models.Cell`) and Blazor component classes (`TicTacToeUI.Components.Cell.Cell`)

## Out of Scope

- No backend integration (GameService class library not used in frontend)
- No AI opponent
- No online multiplayer
- No game history or persistence
- No animations or sound effects
