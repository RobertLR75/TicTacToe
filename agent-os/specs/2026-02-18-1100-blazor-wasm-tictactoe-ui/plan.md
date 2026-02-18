# Blazor WebAssembly TicTacToe UI

## Context

The TicTacToe project has a well-configured backend skeleton and comprehensive Agent-OS standards, but no game logic and no frontend. The user wants to build a Blazor WebAssembly frontend for local 2-player TicTacToe (two players on the same screen, taking turns). The spec folder documents this shaping work, then implementation creates the project from scratch.

Spec folder: `agent-os/specs/2026-02-18-1100-blazor-wasm-tictactoe-ui/`

---

## Task 1: Save Spec Documentation

Create `agent-os/specs/2026-02-18-1100-blazor-wasm-tictactoe-ui/` with:

- `plan.md` — this full plan
- `shape.md` — shaping notes (scope, decisions, context)
- `standards.md` — relevant standards applied
- `references.md` — no existing references (greenfield)

---

## Task 2: Create Blazor WebAssembly Project

Create `src/FrontEnd/TicTacToeUI/TicTacToeUI.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

Add project to `TicTacToe.sln`:
```bash
dotnet sln TicTacToe.sln add src/FrontEnd/TicTacToeUI/TicTacToeUI.csproj
```

Root namespace: `TicTacToeUI` (matches project name per csharp-namespaces standard).

---

## Task 3: Build Game Models

Location: `src/FrontEnd/TicTacToeUI/Models/`

Per `backend/csharp-type-design`: records for value objects, classes for entities.

Files to create:
- `Models/PlayerMark.cs` — `enum PlayerMark { None, X, O }`
- `Models/Cell.cs` — `record Cell(int Row, int Col, PlayerMark Mark)`
- `Models/Move.cs` — `record Move(int Row, int Col, PlayerMark Player)`
- `Models/Board.cs` — `class Board` with 3×3 `Cell[,]` grid, `IsEmpty`, `GetCell`, `SetCell` methods
- `Models/GameState.cs` — `class GameState` with `Board`, `CurrentPlayer`, `Winner`, `IsDraw`, `IsOver` properties

Per `backend/csharp-file-structure`: file-scoped namespaces, one type per file.

---

## Task 4: Build GameService (Frontend Logic)

Location: `src/FrontEnd/TicTacToeUI/Services/GameService.cs`

Namespace: `TicTacToeUI.Services`

Responsibilities:
- `StartNewGame()` — reset board, set X as first player
- `MakeMove(int row, int col)` — place mark, detect win/draw, advance turn
- `CheckWinner()` — rows, columns, diagonals
- `CheckDraw()` — all cells filled, no winner
- Expose `GameState CurrentState { get; private set; }`

Register as scoped in `Program.cs`: `builder.Services.AddScoped<GameService>();`

---

## Task 5: Build Blazor Components

Location: `src/FrontEnd/TicTacToeUI/Components/`

Per `razor-pages/folder-structure` adapted to Blazor: each component in its own folder with co-located files.

**`Components/Cell/Cell.razor`**
- Parameters: `Cell CellData`, `EventCallback<Cell> OnClick`
- Displays X, O, or empty based on `CellData.Mark`
- Disabled when cell is taken or game is over

**`Components/Board/Board.razor`**
- Parameter: `GameState State`, `EventCallback<Cell> OnCellClick`
- Renders 3×3 grid of `Cell` components
- Applies CSS class for win highlight (future-friendly)

---

## Task 6: Build Game Page

Location: `src/FrontEnd/TicTacToeUI/Pages/Game/Index.razor`

Namespace: `TicTacToeUI.Pages.Game`

- `@inject GameService Game`
- Shows current player turn (`Player X's turn`)
- Shows winner / draw message when game ends
- "New Game" button resets state
- Wires `OnCellClick` to `GameService.MakeMove()`
- Calls `StateHasChanged()` after each move

---

## Task 7: Wire Up App Shell

Files to create:
- `wwwroot/index.html` — HTML entry point for Blazor WASM (loads `blazor.webassembly.js`)
- `App.razor` — Router with `Found`/`NotFound` templates
- `MainLayout.razor` / `MainLayout.razor.css` — Shared layout with page title
- `_Imports.razor` — Global `@using` directives (`TicTacToeUI.Components.Board`, `TicTacToeUI.Components.Cell`, etc.)
- `Program.cs` — Blazor WASM entry point: `WebAssemblyHostBuilder`, register `GameService`, `builder.RootComponents.Add<App>("#app")`
- `wwwroot/css/app.css` — Basic board styling (grid, cell hover, X/O colors)

---

## Task 8: Verify Build

```bash
dotnet build TicTacToe.sln
```

Confirms:
- Both `GameService` (class library) and `TicTacToeUI` (Blazor WASM) compile without errors
- No nullable warnings (Nullable is enabled)

To run the frontend:
```bash
dotnet run --project src/FrontEnd/TicTacToeUI/TicTacToeUI.csproj
```
Then open `http://localhost:5000` (or the URL shown) and play a game.

---

## Critical Files

- `TicTacToe.sln` — add new project reference
- `src/FrontEnd/TicTacToeUI/TicTacToeUI.csproj` — new project (create)
- `src/FrontEnd/TicTacToeUI/Models/` — game domain models (create)
- `src/FrontEnd/TicTacToeUI/Services/GameService.cs` — game logic (create)
- `src/FrontEnd/TicTacToeUI/Components/Board/Board.razor` — board UI (create)
- `src/FrontEnd/TicTacToeUI/Components/Cell/Cell.razor` — cell UI (create)
- `src/FrontEnd/TicTacToeUI/Pages/Game/Index.razor` — game page (create)
- `src/FrontEnd/TicTacToeUI/Program.cs` — app entry point (create)

## Applied Standards

- `backend/csharp-project-config` — net10.0, Nullable, ImplicitUsings
- `backend/csharp-file-structure` — file-scoped namespaces, one type per file
- `backend/csharp-namespaces` — root namespace `TicTacToeUI`, folders mirror namespaces
- `backend/csharp-nullability` — no `null!` suppression, prefer `??` and `?.`
- `backend/csharp-type-design` — records for `Cell`, `Move`; classes for `Board`, `GameState`
- `razor-pages/folder-structure` — adapted: each component/page in its own folder