## 1. Request Contracts and Handlers

- [x] 1.1 Create `CheckWinner` `IRequest` contract and handler under the `MakeMove` folder.
- [x] 1.2 Create `CheckDraw` `IRequest` contract and handler under the `MakeMove` folder.
- [x] 1.3 Port existing winner and draw logic from `GameLogicService` into the new handlers without changing behavior.

## 2. MakeMove Flow Integration

- [x] 2.1 Update the `MakeMove` orchestration to dispatch winner and draw requests via the mediator/pipeline.
- [x] 2.2 Remove direct `GameLogicService` calls and update constructor dependencies.
- [x] 2.3 Verify move outcome resolution order (winner check before draw check where applicable) matches current behavior.

## 3. Service Removal and Wiring Cleanup

- [x] 3.1 Remove `GameLogicService` implementation file.
- [x] 3.2 Remove `GameLogicService` interface/registrations and any remaining references in DI configuration.
- [x] 3.3 Run a repository-wide usage check to ensure no `GameLogicService` dependency remains.

## 4. Test Coverage and Validation

- [x] 4.1 Add/adjust unit tests for winner request handler across winning and non-winning board states.
- [x] 4.2 Add/adjust unit tests for draw request handler across full-board draw and incomplete-board scenarios.
- [x] 4.3 Update `MakeMove` flow tests to validate request-based outcome orchestration and behavior parity.
- [x] 4.4 Run unit/integration tests relevant to move handling and fix regressions.
