# Design: Fix CreateGameResponse Missing Fields

## Context

The three response DTOs (`CreateGameResponse`, `GetGameResponse`, `MakeMoveResponse`) were supposed to share the same shape but `CreateGameResponse` was created without the game-over fields. This caused a silent deserialization failure in the frontend.

## Goals / Non-Goals

**Goals:**
- Make `CreateGameResponse` structurally consistent with `GetGameResponse`
- Fix frontend deserialization failure on new game

**Non-Goals:**
- Unifying the three response types into a shared base class (out of scope)
- Removing duplicate `CellDto` definitions across namespaces (separate concern)

## Decisions

### Decision: Add fields directly to CreateGameResponse

Add `Winner`, `IsDraw`, and `IsOver` to `CreateGameResponse` to match `GetGameResponse`. This is the minimal, additive fix — no structural changes needed.

`IsOver` is a computed property on `GameState` (`Winner != None || IsDraw`), so the endpoint reads it directly from the game object.