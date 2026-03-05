## 1. Project Scaffolding

- [x] 1.1 Create `src/Backend/GameNotificationService/` directory and `GameNotificationService.csproj` targeting `net10.0` with `FastEndpoints`, `FastEndpoints.Swagger`, and a project reference to `TicTacToe.ServiceDefaults`
- [x] 1.2 Create `Program.cs` with `AddServiceDefaults()`, FastEndpoints, Swagger (dev only), CORS, and `MapDefaultEndpoints()` -- following the `GameStateService` pattern

## 2. Stub Endpoint

- [x] 2.1 Create a `GET /api/notifications` FastEndpoint that returns an empty JSON array (`[]`) with `200 OK`

## 3. Aspire Integration

- [x] 3.1 Add a project reference for `GameNotificationService` to `TicTacToe.AppHost.csproj`
- [x] 3.2 Register `GameNotificationService` in `AppHost.cs` via `AddProject<Projects.GameNotificationService>("gamenotificationservice")` with `.WithHttpEndpoint(port: 5130, name: "gamenotificationservice-http")`

## 4. Solution Registration

- [x] 4.1 Add `GameNotificationService` to `TicTacToe.sln` under the `src/Backend/` solution folder

## 5. Verification

- [x] 5.1 Run `dotnet build` on the solution and confirm zero errors
- [x] 5.2 Verify the `/health` endpoint responds with `200 OK` when the service starts
- [x] 5.3 Verify the `GET /api/notifications` stub endpoint returns an empty JSON array
