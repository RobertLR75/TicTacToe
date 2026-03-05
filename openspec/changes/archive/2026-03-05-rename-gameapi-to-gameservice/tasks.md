## 1. Filesystem Rename

- [x] 1.1 Rename directory `src/Backend/GameApi/` to `src/Backend/GameService/`
- [x] 1.2 Rename `GameApi.csproj` to `GameService.csproj` within the renamed directory
- [x] 1.3 Clean `bin/` and `obj/` directories in the renamed project

## 2. Namespace Updates

- [x] 2.1 Replace all `GameApi` namespace and using references with `GameService` in `Program.cs`
- [x] 2.2 Replace all `GameApi` namespace and using references with `GameService` in `Services/GameRepository.cs`
- [x] 2.3 Replace all `GameApi` namespace references with `GameService` in `Models/GameModel.cs`, `Models/PlayerModel.cs`, and `Models/GameStatus.cs`
- [x] 2.4 Replace all `GameApi` namespace and using references with `GameService` in all endpoint files under `Endpoints/Games/` (Create, Activate, Complete, List)

## 3. Solution and AppHost References

- [x] 3.1 Update `TicTacToe.sln` to reference `GameService` at `src\Backend\GameService\GameService.csproj`
- [x] 3.2 Update `src/TicTacToe.AppHost/TicTacToe.AppHost.csproj` project reference from `GameApi\GameApi.csproj` to `GameService\GameService.csproj`
- [x] 3.3 Update `src/TicTacToe.AppHost/AppHost.cs`: change `Projects.GameApi` to `Projects.GameService`, resource name to `"gameservice"`, endpoint name to `"gameservice-http"`, and variable name to `gameservice`

## 4. Verification

- [x] 4.1 Build the full solution (`dotnet build TicTacToe.sln`) and verify zero errors
- [x] 4.2 Verify all `.cs` files in `src/Backend/GameService/` use `GameService` as root namespace with no remaining `GameApi` references
