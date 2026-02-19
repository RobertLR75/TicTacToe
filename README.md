# TicTacToe - Blazor WebAssembly med API Backend

Dette prosjektet består av to deler:
- **Backend API** (GameService) - Håndterer all spillogikk
- **Frontend UI** (TicTacToeUI) - Blazor WebAssembly-applikasjon

## Arkitektur

All spillogikk ligger nå i backend API-et. Frontend sender HTTP-forespørsler til API-et for:
- Opprette nytt spill
- Hente spillstatus
- Gjøre trekk

## ⚡ Hurtigstart

### 🌟 Anbefalt: Bruk Aspire AppHost (1 kommando!)

**Start både backend og frontend automatisk med Aspire:**

```bash
./start-aspire.sh
```

Dette starter **Aspire AppHost** som kjører:
- **Backend API**: https://localhost:7082
- **Frontend UI**: https://localhost:7080

**Fordeler:**
- ✅ Én kommando starter alt
- ✅ Backend bruker ServiceDefaults (OpenTelemetry, health checks)
- ✅ Begge tjenester kjører som executables (unngår runtime-konflikter)
- ✅ Stopp alt med Ctrl+C

> **💡 Merk:** AppHost kjører begge prosjektene som executables for å unngå `browser-wasm` runtime-konflikter med Blazor WebAssembly.

---

### Alternativ: Manuell oppstart

**Første gang eller ved problemer:**

**1. Setup (rens og bygg):**
```bash
./setup.sh
```

**2. Start Backend (Terminal 1):**
```bash
./start-backend.sh
```

**3. Start Frontend (Terminal 2):**
```bash
./start-frontend.sh
```

**4. Åpne nettleseren:**
```bash
open https://localhost:7080
```
Deretter: **Cmd+Shift+R** (hard refresh)

### Ved PDB-feil eller build-problemer:
```bash
./cleanup.sh  # Rens alt
./setup.sh    # Bygg på nytt
```

Se [QUICKSTART.md](QUICKSTART.md) for detaljert feilsøking.

---

## Hvordan starte applikasjonen (Manuelt)

### 1. Start Backend API (Terminal 1)

```bash
cd src/Backend/GameService
dotnet run
```

API-et kjører på:
- HTTPS: https://localhost:7082
- HTTP: http://localhost:5082
- Swagger: https://localhost:7082/swagger

### 2. Start Frontend UI (Terminal 2)

```bash
cd src/FrontEnd/TicTacToeUI
dotnet run
```

Frontend kjører på:
- HTTPS: https://localhost:7080
- HTTP: http://localhost:5080

### 3. Åpne nettleseren

Naviger til: **https://localhost:7080**

## API Endpoints

### POST /api/games
Oppretter et nytt spill.

**Response:**
```json
{
  "gameId": "guid",
  "currentPlayer": "X",
  "winner": "None",
  "isDraw": false,
  "isOver": false,
  "board": [
    { "row": 0, "col": 0, "mark": "None" },
    ...
  ]
}
```

### GET /api/games/{gameId}
Henter spillstatus.

**Response:** (samme som Create)

### POST /api/games/{gameId}/moves
Gjør et trekk.

**Request:**
```json
{
  "gameId": "guid",
  "row": 0,
  "col": 0
}
```

**Response:** (samme som Create)

## Teknologier

### Backend
- .NET 10
- FastEndpoints
- In-memory storage (ConcurrentDictionary)

### Frontend
- Blazor WebAssembly
- .NET 10
- HttpClient for API-kommunikasjon

## Struktur

### Backend (GameService)
```
src/Backend/GameService/
├── Endpoints/
│   └── Games/
│       ├── Create/
│       ├── Get/
│       └── MakeMove/
├── Models/
│   ├── Board.cs
│   ├── Cell.cs
│   ├── GameState.cs
│   └── PlayerMark.cs
├── Services/
│   ├── GameLogicService.cs    # Spillogikk (vinner, uavgjort)
│   └── GameRepository.cs       # In-memory lagring
└── Program.cs
```

### Frontend (TicTacToeUI)
```
src/FrontEnd/TicTacToeUI/
├── Components/
│   ├── Board/
│   └── Cell/
├── Models/
│   ├── Board.cs
│   ├── Cell.cs
│   ├── GameState.cs
│   └── PlayerMark.cs
├── Pages/
│   └── Game/
│       └── Index.razor
├── Services/
│   ├── GameApiClient.cs        # HTTP klient for API
│   └── GameService.cs          # Frontend service (bruker API)
└── Program.cs
```

## CORS

Backend API-et er konfigurert med CORS for å tillate forespørsler fra:
- https://localhost:7080
- http://localhost:5080

## Utviklingstips

- Bruk Swagger UI på https://localhost:7081/swagger for å teste API-et
- Åpne nettleserens developer tools (F12) for å se nettverkstrafikk
- Backend logger alle forespørsler i konsollen

