## 1. AppHost Orchestration Alignment

- [x] 1.1 Update `src/TicTacToe.AppHost/AppHost.cs` to ensure `gamestateservice` is explicitly registered as an Aspire-managed project with required endpoint naming and infrastructure references.
- [x] 1.2 Update `src/TicTacToe.AppHost/AppHost.cs` to ensure `gamenotificationservice` is explicitly registered with required references and dependency waits for PostgreSQL and RabbitMQ.

## 2. Frontend Service Reference Consistency

- [x] 2.1 Verify `frontend` AppHost registration references `gamestateservice` and `gamenotificationservice` resources required for gameplay and real-time updates.
- [x] 2.2 Remove or adjust any obsolete/misaligned AppHost references so resource names match actual service discovery usage.

## 3. Validation

- [x] 3.1 Run AppHost build/start validation (`dotnet build` and targeted AppHost run checks) to confirm services are orchestrated and healthy.
- [x] 3.2 Confirm no regression to existing frontend orchestration behavior and capture any follow-up adjustments needed for service startup sequencing.
