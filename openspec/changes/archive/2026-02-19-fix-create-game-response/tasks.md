# Tasks: Fix CreateGameResponse Missing Fields

## 1. CreateGameResponse.cs

- [x] 1.1 Add `Winner` property (`PlayerMark`, required)
- [x] 1.2 Add `IsDraw` property (`bool`, required)
- [x] 1.3 Add `IsOver` property (`bool`, required)

## 2. CreateGameEndpoint.cs

- [x] 2.1 Populate `Winner`, `IsDraw`, and `IsOver` from the game state in the response

## 3. Verify

- [x] 3.1 Build the solution — no errors
- [x] 3.2 Confirm `CreateGameResponse` fields match `GetGameResponse`