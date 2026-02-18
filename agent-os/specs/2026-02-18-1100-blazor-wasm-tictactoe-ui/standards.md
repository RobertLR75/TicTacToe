# Standards Applied

- `backend/csharp-project-config` — net10.0, Nullable enable, ImplicitUsings enable
- `backend/csharp-file-structure` — file-scoped namespaces, one type per file
- `backend/csharp-namespaces` — root namespace `TicTacToeUI`, folders mirror namespaces
- `backend/csharp-nullability` — no `null!` suppression; `default!` only where Blazor parameter binding requires
- `backend/csharp-type-design` — records for `Cell`, `Move`; classes for `Board`, `GameState`
- `razor-pages/folder-structure` — adapted: each component/page in its own named folder
