## ADDED Requirements

### Requirement: Game logic operations SHALL use IRequest handlers
GameStateService SHALL model game-logic operations as request/handler pairs where each operation is represented by an `IRequest<TResponse>` and executed by a corresponding `HandleAsync` handler.

#### Scenario: Move operation modeled as request
- **WHEN** move processing is executed
- **THEN** the operation SHALL be represented by a typed request model carrying the required move inputs
- **AND** a corresponding handler SHALL produce the operation outcome via `HandleAsync`

### Requirement: Game logic request handlers SHALL preserve existing game rule outcomes
Game logic request handlers SHALL preserve existing move validation and state transition behavior implemented by the current game rules.

#### Scenario: Invalid move remains rejected
- **WHEN** a move request targets an occupied cell or a completed game
- **THEN** the handler SHALL return the same rejection outcome category as before the refactor

#### Scenario: Valid move applies same transitions
- **WHEN** a valid move request is processed
- **THEN** the handler SHALL apply board updates, winner/draw evaluation, and current-player transitions equivalent to current behavior

### Requirement: Game logic handlers SHALL be integrated through dependency injection
GameStateService SHALL register game-logic request handlers with dependency injection and consume them from orchestration call sites instead of direct imperative method calls.

#### Scenario: Handler resolved at runtime
- **WHEN** a game-state orchestration path runs in the application
- **THEN** required game-logic handlers SHALL be resolved from the dependency injection container
- **AND** direct logic invocation paths replaced by this change SHALL no longer be the primary execution path
