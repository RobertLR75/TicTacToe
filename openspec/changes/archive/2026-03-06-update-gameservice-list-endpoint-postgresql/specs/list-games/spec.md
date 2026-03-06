## ADDED Requirements

### Requirement: Game list endpoint uses PostgreSQL shared search

The `GET /api/games` endpoint SHALL retrieve game list data through `SharedLibrary.PostgreSQL` using `SearchAsync` and a list-specific specification object.

#### Scenario: Endpoint uses specification-based search
- **WHEN** a client calls `GET /api/games`
- **THEN** GameService SHALL execute list retrieval via `SearchAsync`
- **AND** GameService SHALL provide a specification that encapsulates filtering, paging, and ordering inputs

### Requirement: Game list filters and paging are preserved

The list specification SHALL preserve existing list semantics for supported filters, sorting, and pagination so public API behavior remains backward compatible.

#### Scenario: Filtered list returns matching games
- **WHEN** a client calls `GET /api/games` with supported filter parameters
- **THEN** the endpoint SHALL return only games matching those filters
- **AND** filtering behavior SHALL be equivalent to pre-migration behavior

#### Scenario: Paging behavior remains stable
- **WHEN** a client calls `GET /api/games` with paging parameters
- **THEN** the endpoint SHALL honor requested page and page size semantics
- **AND** the returned page metadata and item boundaries SHALL remain backward compatible

### Requirement: Repository abstraction is removed for game listing

GameService SHALL remove `GameRepository` from the game listing path, and `GameRepository` SHALL be deleted once no runtime references remain.

#### Scenario: List endpoint has no repository dependency
- **WHEN** the application resolves dependencies for the game list flow
- **THEN** no `GameRepository` dependency SHALL be required for handling `GET /api/games`

#### Scenario: Build succeeds after repository deletion
- **WHEN** `GameRepository` is removed from the codebase
- **THEN** the solution SHALL build and tests for list retrieval SHALL pass with the specification-based search path
