
# Copilot Project Instructions

This repository follows a distributed .NET architecture.

Stack:
- .NET 8+
- FastEndpoints
- Vertical Slice Architecture
- CQRS
- Blazor + MudBlazor
- Redis
- PostgreSQL / SQL Server
- MongoDB
- RabbitMQ or Azure Service Bus
- .NET Aspire

Rules:

- APIs must use FastEndpoints
- Feature slices must follow Vertical Slice Architecture
- Domain logic must live in domain models
- Services communicate via integration events
- Databases must not be shared between services
- Tests must be generated for all new features
