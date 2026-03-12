# gameservice-persistence-configuration Specification

## Purpose
Define how GameService resolves PostgreSQL persistence startup configuration from supported runtime sources.

## Requirements
### Requirement: GameService resolves PostgreSQL persistence configuration from supported runtime sources

GameService SHALL resolve the PostgreSQL connection required for persistence startup from supported runtime configuration sources instead of requiring only `ConnectionStrings:postgres`.

#### Scenario: Supported runtime configuration starts GameService persistence

- **WHEN** GameService starts in an environment where PostgreSQL configuration is supplied through a supported runtime configuration source
- **THEN** persistence registration SHALL resolve a usable PostgreSQL connection
- **AND** GameService SHALL complete startup without throwing a missing-configuration exception

### Requirement: Missing persistence configuration fails with actionable guidance

GameService SHALL fail fast with a clear actionable error when no supported PostgreSQL configuration source provides a usable connection for persistence.

#### Scenario: Missing configuration reports actionable startup error

- **WHEN** GameService starts and no supported PostgreSQL configuration source provides a usable connection
- **THEN** startup SHALL fail before serving requests
- **AND** the thrown error SHALL explain that PostgreSQL configuration is required
- **AND** the error SHALL identify the supported configuration path or paths that can satisfy the requirement

### Requirement: Configuration source precedence is deterministic

GameService SHALL apply a deterministic precedence order when more than one supported PostgreSQL configuration source is present.

#### Scenario: Preferred configuration source wins consistently

- **WHEN** multiple supported PostgreSQL configuration sources are present at startup
- **THEN** GameService SHALL choose the connection using a documented precedence order
- **AND** the same input set SHALL always produce the same selected connection
