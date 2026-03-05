## Why

The TicTacToeMud frontend is referenced in the Aspire AppHost project but never registered with `AddProject`, so it isn't managed by Aspire. This means the frontend misses out on Aspire's orchestration, service discovery, OpenTelemetry, and health checks — all of which the GameService backend already benefits from.

## What Changes

- Register `TicTacToeMud` in the Aspire `AppHost.cs` via `builder.AddProject` with a reference to the `gameservice` resource
- Add `ServiceDefaults` project reference to `TicTacToeMud.csproj`
- Wire up `AddServiceDefaults()` and `MapDefaultEndpoints()` in the TicTacToeMud `Program.cs`

## Capabilities

### New Capabilities
- `aspire-frontend-orchestration`: Aspire manages the TicTacToeMud frontend lifecycle, providing service discovery, OpenTelemetry, and health checks

### Modified Capabilities
<!-- None — no existing spec-level behavior is changing -->

## Impact

- `src/TicTacToe.AppHost/AppHost.cs` — add `TicTacToeMud` project registration
- `src/FrontEnd/TicTacToeMud/TicTacToeMud.csproj` — add ServiceDefaults reference
- `src/FrontEnd/TicTacToeMud/Program.cs` — add ServiceDefaults and health endpoint calls
