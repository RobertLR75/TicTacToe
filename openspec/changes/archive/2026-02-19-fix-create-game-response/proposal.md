# Proposal: Fix CreateGameResponse Missing Fields

## Why

`CreateGameResponse` omits three fields (`Winner`, `IsDraw`, `IsOver`) that are marked `required` in the frontend's `GameStateDto`. When the frontend calls `POST /api/games` and tries to deserialize the response, deserialization returns `null`, silently preventing any game from starting.

## What Changes

- `CreateGameResponse` gains `Winner`, `IsDraw`, and `IsOver` properties
- `CreateGameEndpoint` populates those properties from the game state

## Capabilities

### Modified Capabilities

- `create-game`: Response now returns the full game state shape, consistent with `GetGameResponse` and `MakeMoveResponse`

## Impact

- `src/Backend/GameService/Endpoints/Games/Create/CreateGameResponse.cs`: add 3 fields
- `src/Backend/GameService/Endpoints/Games/Create/CreateGameEndpoint.cs`: populate 3 fields