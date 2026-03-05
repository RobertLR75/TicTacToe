## 1. Wire TicTacToeMud into ServiceDefaults

- [x] 1.1 Add `ProjectReference` to `TicTacToe.ServiceDefaults` in `TicTacToeMud.csproj`
- [x] 1.2 Add `builder.AddServiceDefaults()` and `app.MapDefaultEndpoints()` in `TicTacToeMud/Program.cs`

## 2. Register in Aspire AppHost

- [x] 2.1 Register `TicTacToeMud` via `builder.AddProject` with `.WithReference(gameservice)` in `AppHost.cs`

## 3. Verify

- [x] 3.1 Build the solution to confirm everything compiles
