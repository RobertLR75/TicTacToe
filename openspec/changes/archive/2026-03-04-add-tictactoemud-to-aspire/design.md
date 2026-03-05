## Context

GameService is already Aspire-managed: registered in AppHost.cs, uses ServiceDefaults for OpenTelemetry/health checks. TicTacToeMud has a project reference in the AppHost .csproj but is not registered in the distributed application builder, and does not reference ServiceDefaults.

## Goals / Non-Goals

**Goals:**
- TicTacToeMud is managed by Aspire (starts/stops with the AppHost, visible in the Aspire dashboard)
- TicTacToeMud gets OpenTelemetry and health checks via ServiceDefaults
- Frontend has a `WithReference` to gameservice for future service discovery

**Non-Goals:**
- Actually consuming the GameService API from the frontend (separate task)
- Fixing the CORS origin mismatch in GameService (separate task)
- Adding an explicit HTTP endpoint override — let Aspire assign ports dynamically

## Decisions

### Decision 1: Follow the GameService pattern exactly

Use `builder.AddProject<Projects.TicTacToeMud>(...)` with `.WithReference(gameservice)` to establish the dependency. Wire ServiceDefaults the same way GameService does: `builder.AddServiceDefaults()` before building, `app.MapDefaultEndpoints()` after building.

**Rationale:** Consistency across services. One pattern to understand, one pattern to maintain.

### Decision 2: Let Aspire manage ports

Don't call `.WithHttpEndpoint(port: ..., ...)` for the frontend. Let Aspire assign ports dynamically. The launchSettings.json standalone ports remain for running outside Aspire.

**Rationale:** Fewer hardcoded ports to manage. Aspire's dynamic port assignment is the recommended pattern for services that don't need a stable external port.

## Risks / Trade-offs

- [Port conflict] If TicTacToeMud's standalone launchSettings port clashes with an Aspire-assigned port → Aspire will pick a different port, no action needed.
- [CORS mismatch] GameService CORS allows `localhost:5081` which won't match the Aspire-assigned port → Out of scope for this change; noted for follow-up.
