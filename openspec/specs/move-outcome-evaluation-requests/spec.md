# move-outcome-evaluation-requests Specification

## Purpose
Defines request-based winner and draw outcome evaluation in `MakeMove`, replacing direct `GameLogicService` checks.

## Requirements

### Requirement: MakeMove winner evaluation SHALL be handled through IRequest
The system MUST evaluate winning game states in the `MakeMove` flow by dispatching a dedicated `IRequest` handler instead of calling `GameLogicService.CheckWinner` directly.

#### Scenario: Winning move is resolved through request handler
- **GIVEN** a game state where the current move completes a winning line
- **WHEN** the `MakeMove` flow evaluates winner status
- **THEN** winner evaluation SHALL execute via the winner-check request handler
- **THEN** the resulting winner outcome SHALL match the previous behavior for the same board state

#### Scenario: Non-winning move returns no winner via request handler
- **GIVEN** a game state where no winning line exists after the move
- **WHEN** the `MakeMove` flow evaluates winner status
- **THEN** the winner-check request handler SHALL return a no-winner result

### Requirement: MakeMove draw evaluation SHALL be handled through IRequest
The system MUST evaluate draw game states in the `MakeMove` flow by dispatching a dedicated `IRequest` handler instead of calling `GameLogicService.CheckDraw` directly.

#### Scenario: Full board with no winner is resolved as draw
- **GIVEN** a game state with a full board and no winner
- **WHEN** the `MakeMove` flow evaluates draw status
- **THEN** draw evaluation SHALL execute via the draw-check request handler
- **THEN** the resulting draw outcome SHALL match the previous behavior for the same board state

#### Scenario: Incomplete board is not resolved as draw
- **GIVEN** a game state with at least one empty cell and no winner
- **WHEN** the `MakeMove` flow evaluates draw status
- **THEN** the draw-check request handler SHALL return a not-draw result

### Requirement: GameLogicService SHALL be removed after request migration
The system MUST not depend on `GameLogicService` for move outcome evaluation once winner and draw checks are migrated to request handlers.

#### Scenario: MakeMove has no GameLogicService dependency
- **GIVEN** the refactor is applied
- **WHEN** the `MakeMove` handler dependencies are inspected
- **THEN** `GameLogicService` SHALL not be required or injected

#### Scenario: Example request-based orchestration pattern
- **GIVEN** the implementation follows existing mediator conventions
- **WHEN** developers review move orchestration
- **THEN** winner and draw checks SHALL be represented as request dispatches, for example:

```csharp
var winner = await mediator.Send(new CheckWinnerRequest(gameState), cancellationToken);
var isDraw = await mediator.Send(new CheckDrawRequest(gameState), cancellationToken);
```
