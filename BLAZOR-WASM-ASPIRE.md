# ⚠️ Blazor WebAssembly og Aspire AppHost

## Problemet

Blazor WebAssembly-prosjekter bruker `browser-wasm` som RuntimeIdentifier, som ikke er kompatibel med .NET Aspire AppHost.

Når du prøver å referere et Blazor WASM-prosjekt i AppHost, får du denne feilen:

```
NETSDK1082: There was no runtime pack for Microsoft.AspNetCore.App 
available for the specified RuntimeIdentifier 'browser-wasm'.
```

## Hvorfor skjer dette?

- **Blazor WASM** kjører i nettleseren (WebAssembly runtime)
- **Aspire AppHost** forventer .NET server-side prosjekter
- De har inkompatible runtime targets

## Løsninger

### ✅ Løsning: Bruk AddExecutable for BEGGE tjenestene (implementert)

```csharp
// Backend API - run as executable
var backendPath = Path.Combine("..", "Backend", "GameService");
builder.AddExecutable("gameservice", "dotnet", backendPath, "run", "--no-build")
    .WithHttpsEndpoint(port: 7082, name: "https")
    .WithHttpEndpoint(port: 5082, name: "http")
    .WithEnvironment("ASPNETCORE_URLS", "https://localhost:7082;http://localhost:5082");

// Frontend Blazor WASM - run as executable
var frontendPath = Path.Combine("..", "FrontEnd", "TicTacToeUI");
builder.AddExecutable("frontend", "dotnet", frontendPath, "run", "--no-build")
    .WithHttpsEndpoint(port: 7080, name: "https")
    .WithHttpEndpoint(port: 5080, name: "http")
    .WithEnvironment("ASPNETCORE_URLS", "https://localhost:7080;http://localhost:5080");
```

**Fordeler:**
- ✅ Ingen NETSDK1082 browser-wasm errors
- ✅ Ingen compile-time dependencies
- ✅ Backend får fortsatt ServiceDefaults (OpenTelemetry, health checks)
- ✅ Enkel og pålitelig
- ✅ Prosjektene bygges først, deretter kjøres med --no-build

## Hva TicTacToe bruker

Vi bruker **Løsning 2** fordi:

✅ Blazor WASM gir bedre ytelse (kjører i nettleseren)
✅ Ingen server-side state
✅ Backend får fortsatt alle Aspire-fordeler (OpenTelemetry, health checks)
✅ Enkel deployment (static files for frontend)

## Backend får fortsatt Aspire-fordeler

Selv om frontend ikke kjører i AppHost, får backend fortsatt:

- ✅ **ServiceDefaults** - OpenTelemetry, health checks, resilience
- ✅ **Observability** - Structured logging
- ✅ **Health endpoints** - `/health` og `/alive`
- ✅ **Metrics** - HTTP metrics via OpenTelemetry

## Fremtidige muligheter

Microsoft jobber med bedre støtte for Blazor WASM i Aspire:
- Static web app hosting
- Better executable integration
- Proxy configuration for dev server

## Konklusjon

For nå er den beste løsningen å:
1. Bruke ServiceDefaults i backend for observability
2. Starte Blazor WASM separat som en standard dev server
3. Bruke `./start-aspire.sh` for enkel oppstart av begge

**Backend får alle Aspire-fordelene, mens frontend kjører optimalt som WebAssembly! 🎯**

## Referanser

- [Aspire + Blazor WASM Discussion](https://github.com/dotnet/aspire/issues/xxxx)
- [RuntimeIdentifier Documentation](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog)
- [Blazor Hosting Models](https://learn.microsoft.com/en-us/aspnet/core/blazor/hosting-models)

