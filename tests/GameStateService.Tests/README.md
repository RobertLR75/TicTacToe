# GameStateService.Tests

## Test Groups

- Unit tests: `*UnitTests.cs`
- Integration tests (RabbitMQ/Testcontainers): `*IntegrationTests.cs`

## Local Execution

- Run all tests:
  - `dotnet test tests/GameStateService.Tests/GameStateService.Tests.csproj`
- Run unit tests only:
  - `dotnet test tests/GameStateService.Tests/GameStateService.Tests.csproj --filter "FullyQualifiedName~UnitTests"`
- Run integration tests only:
  - `dotnet test tests/GameStateService.Tests/GameStateService.Tests.csproj --filter "FullyQualifiedName~IntegrationTests"`

## CI Assumptions

- Docker must be available to execute Testcontainers-based integration tests.
- Integration tests should run in a clean environment to avoid queue name collisions and stale broker state.
- If needed for faster PR feedback, use unit-test filter on PRs and run full integration suite on main/nightly.
