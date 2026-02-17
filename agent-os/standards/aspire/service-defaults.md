# Service Defaults

Every service project must reference `IdentityServer.ServiceDefaults` and call `AddServiceDefaults()` as early as possible in `Program.cs`.

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();  // first, before other registrations
// ... rest of DI setup
```

`AddServiceDefaults()` provides:
- OpenTelemetry (logging, metrics, tracing)
- Health checks (`/health`, `/alive`)
- Service discovery
- HTTP client resilience (standard resilience handler + service discovery on all `HttpClient`s)

Also call `app.MapDefaultEndpoints()` in the pipeline to expose health check routes.