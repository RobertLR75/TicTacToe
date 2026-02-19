# 🌟 .NET Aspire Setup for TicTacToe

## Hva er .NET Aspire?

.NET Aspire er en opinionated stack for å bygge observerbare, production-ready cloud-native applikasjoner med .NET. Det gir:

- **Orkesterering** - Start alle tjenester med én kommando
- **Service Discovery** - Automatisk service-to-service kommunikasjon
- **Observability** - Logging, metrics, og distributed tracing out-of-the-box
- **Resilience** - Retry policies, circuit breakers, timeout policies
- **Dashboard** - Visuell oversikt over alle tjenester

> **📦 NuGet-basert:** Aspire bruker nå NuGet-pakker i stedet for workloads. Ingen ekstra SDK-installasjon nødvendig!

## Prosjektstruktur

```
TicTacToe/
├── src/
│   ├── AppHost/                    # Aspire orchestration
│   │   ├── Program.cs             # Definerer tjenester og avhengigheter
│   │   └── AppHost.csproj
│   ├── ServiceDefaults/           # Delte konfigurasjoner
│   │   ├── Extensions.cs          # OpenTelemetry, health checks
│   │   └── ServiceDefaults.csproj
│   ├── Backend/
│   │   └── GameService/           # Backend API (bruker ServiceDefaults)
│   └── FrontEnd/
│       └── TicTacToeUI/           # Blazor WASM (bruker ServiceDefaults)
```

## Hvordan det fungerer

### AppHost (src/AppHost/Program.cs)

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Backend API - run as executable
var backendPath = Path.Combine("..", "Backend", "GameService");
var gameService = builder.AddExecutable("gameservice", "dotnet", backendPath, "run", "--no-build")
    .WithHttpsEndpoint(port: 7082, name: "https")
    .WithHttpEndpoint(port: 5082, name: "http")
    .WithEnvironment("ASPNETCORE_URLS", "https://localhost:7082;http://localhost:5082");

// Frontend Blazor WASM - run as executable
var frontendPath = Path.Combine("..", "FrontEnd", "TicTacToeUI");
builder.AddExecutable("frontend", "dotnet", frontendPath, "run", "--no-build")
    .WithHttpsEndpoint(port: 7080, name: "https")
    .WithHttpEndpoint(port: 5080, name: "http")
    .WithEnvironment("ASPNETCORE_URLS", "https://localhost:7080;http://localhost:5080");

builder.Build().Run();
```

**Dette gjør:**
1. Kjører backend som executable (unngår project reference issues)
2. Kjører frontend som executable (unngår browser-wasm runtime konflikt)
3. Begge bruker `--no-build` (prosjektene bygges først av start-aspire.sh)
4. Setter eksplisitte porter via miljøvariabler

**Hvorfor executables?**
- ✅ Unngår `NETSDK1082` error med browser-wasm runtime
- ✅ Ingen compile-time dependencies mellom AppHost og prosjektene
- ✅ Enklere og mer pålitelig
- ✅ Backend får fortsatt ServiceDefaults-fordeler

> **💡 Viktig:** Prosjektene må bygges FØR AppHost starter (gjøres av `./start-aspire.sh`)

### ServiceDefaults (src/ServiceDefaults/Extensions.cs)

Gir alle tjenester:
- **OpenTelemetry** - Logging, metrics, tracing
- **Health Checks** - `/health` og `/alive` endpoints
- **Resilience** - Retry policies for HTTP-kall
- **Service Discovery** - Finn andre tjenester automatisk

## Starte med Aspire

### 1. Enkel start:

```bash
./start-aspire.sh
```

### 2. Eller manuelt:

```bash
cd src/AppHost
dotnet run
```

## Aspire Dashboard

Dashboard åpnes automatisk på: **https://localhost:17280**

### Funksjoner:

**Resources** - Se alle kjørende tjenester:
- gameservice (Backend API)
- frontend (Blazor UI)
- Status, port, health

**Console** - Sanntids logger fra alle tjenester

**Structured Logs** - Filtrerbare, søkbare logger

**Traces** - Distributed tracing
- Se HTTP-requests mellom frontend og backend
- Finn flaskehalser
- Debug performance-problemer

**Metrics** - Grafer og telemetri
- HTTP request rates
- Response times
- Error rates
- Memory usage
- CPU usage

## Fordeler for TicTacToe

### Før Aspire:
```bash
# Terminal 1
./start-backend.sh

# Terminal 2
./start-frontend.sh

# Manuell koordinering, ingen oversikt
```

### Med Aspire:
```bash
# EN terminal
./start-aspire.sh

# Alt starter automatisk, full oversikt i dashboard
```

### Observability:

**Se HTTP-kall i sanntid:**
- Frontend POST til `/api/games`
- Frontend POST til `/api/games/{id}/moves`
- Backend response times
- Feilmeldinger og stack traces

**Debug enkelt:**
- Klikk på en trace for å se hele request-flow
- Se logger filtrert per tjeneste
- Metrics viser om backend er treg

## Konfigurasjon

### Ports:

- **Aspire Dashboard**: 17280 (HTTPS), 15280 (HTTP)
- **Backend API**: 7082 (HTTPS), 5082 (HTTP)
- **Frontend UI**: 7080 (HTTPS), 5080 (HTTP)

### Environment Variables:

Aspire setter automatisk:
- `OTEL_EXPORTER_OTLP_ENDPOINT` - For telemetri
- `DOTNET_DASHBOARD_OTLP_ENDPOINT_URL` - Dashboard connection
- `DOTNET_RESOURCE_SERVICE_ENDPOINT_URL` - Service discovery

## Health Checks

Alle tjenester har nå:

```
GET /health       - Overall health
GET /alive        - Liveness probe
```

Se status i Aspire Dashboard under "Resources".

## Production

Aspire er designet for både development og production:

**Development:**
- Dashboard for debugging
- Detaljerte logs
- Tracing enabled

**Production:**
- Deploy til Azure Container Apps
- Kubernetes med Aspire manifest
- AWS med Aspire export

## Neste steg

1. Kjør `./start-aspire.sh`
2. Åpne dashboard: https://localhost:17280
3. Åpne spill: https://localhost:7080
4. Spill en runde
5. Se i dashboard:
   - Console logs
   - HTTP traces
   - Metrics

**Aspire gjør det enkelt! 🚀**

