# Health Check Endpoints

Two endpoints, mapped in development only via `app.MapDefaultEndpoints()`:

| Endpoint | Purpose | Passes when |
|----------|---------|-------------|
| `/health` | Readiness | All registered health checks pass |
| `/alive` | Liveness | Only checks tagged `"live"` pass |

Tag a check as liveness-only with `["live"]`:
```csharp
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);
```

- Endpoints are **dev-only** by default — production enablement requires explicit opt-in with appropriate auth/network controls
- Health check paths are excluded from OpenTelemetry tracing to avoid noise