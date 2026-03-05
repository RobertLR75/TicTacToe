## 1. Rename Project Directory and File

- [x] 1.1 Rename directory `src/Backend/GameService/` to `src/Backend/GameStateService/`
- [x] 1.2 Rename project file `GameService.csproj` to `GameStateService.csproj`

## 2. Update Solution File

- [x] 2.1 Update `TicTacToe.sln` to reference `GameStateService` at the new path (`src\Backend\GameStateService\GameStateService.csproj`)

## 3. Update Aspire AppHost

- [x] 3.1 Update project reference in `src/TicTacToe.AppHost/TicTacToe.AppHost.csproj` from `GameService.csproj` to `GameStateService.csproj` at the new path
- [x] 3.2 Update `src/TicTacToe.AppHost/AppHost.cs` to use `Projects.GameStateService` and update resource name to `"gamestateservice"`

## 4. Update Namespaces in Source Files

- [x] 4.1 Update `Program.cs` — change `using GameService.Services` to `using GameStateService.Services`
- [x] 4.2 Update all files in `Models/` — change `namespace GameService.Models` to `namespace GameStateService.Models`
- [x] 4.3 Update all files in `Services/` — change `namespace GameService.Services` and `using GameService.Models` to `GameStateService` equivalents
- [x] 4.4 Update all files in `Endpoints/Games/Create/` — change all `using GameService.*` and `namespace GameService.*` to `GameStateService` equivalents
- [x] 4.5 Update all files in `Endpoints/Games/Get/` — change all `using GameService.*` and `namespace GameService.*` to `GameStateService` equivalents
- [x] 4.6 Update all files in `Endpoints/Games/MakeMove/` — change all `using GameService.*` and `namespace GameService.*` to `GameStateService` equivalents

## 5. Verify

- [x] 5.1 Run `dotnet build TicTacToe.sln` and confirm zero errors
- [x] 5.2 Verify no remaining references to the old `GameService` namespace in source files (search for `namespace GameService` and `using GameService`)
