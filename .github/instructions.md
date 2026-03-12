
# Copilot Project Instructions

This repository implements a **distributed .NET platform** built with modern cloud-native architecture.

Copilot must follow these architectural rules when generating code.

---

# Technology Stack

The platform uses the following technologies:

- .NET 8+
- FastEndpoints
- Vertical Slice Architecture
- CQRS (when appropriate)
- Domain Driven Design
- Blazor + MudBlazor
- Redis (distributed cache)
- PostgreSQL or SQL Server (transactional storage)
- MongoDB (read models / projections)
- RabbitMQ or Azure Service Bus (messaging)
- .NET Aspire (service orchestration)
- OpenTelemetry (observability)

---

# Architecture Principles

Copilot must always prioritize:

- **Vertical Slice Architecture**
- **Domain Driven Design**
- **Event-driven communication**
- **Loose coupling between services**
- **Testable code**

Avoid generating:

- layered monolithic architectures
- controllers with business logic
- shared databases between services
- tightly coupled services

---

# API Design

All APIs must use **FastEndpoints**.

Endpoints must follow this structure:

Rules:

- Endpoints must remain **thin**
- Business logic belongs in **handlers**
- Validation must use **FluentValidation**
- DTOs must not expose domain entities

---

# Domain Model

Domain logic must live in **domain models**, not services or endpoints.

Domain layer should contain:

- Aggregates
- Entities
- Value Objects
- Domain Events

Rules:

- Aggregates enforce business invariants
- Value objects must be immutable
- Domain models must remain persistence-agnostic

---

# Persistence Strategy

The system uses **polyglot persistence**.

| Technology | Usage |
|------------|-------|
PostgreSQL / SQL Server | transactional services |
MongoDB | read models |
Redis | caching and distributed state |

Rules:

- Each service **owns its database**
- Cross-service database access is forbidden
- EF Core should be used for write models
- Dapper may be used for read models

---

# Messaging Architecture

Services communicate using **asynchronous messaging**.

Supported brokers:

- RabbitMQ
- Azure Service Bus

Messaging rules:

- Events represent **facts that already occurred**
- Integration events must be versionable
- Consumers must be **idempotent**

Reliability patterns:

- Outbox pattern
- Retry policies
- Dead letter queues

---

# Frontend Guidelines

Frontend applications use:

- Blazor
- MudBlazor components

Rules:

- Use MudBlazor components instead of custom UI controls
- Do not call HttpClient directly from components
- Use typed API services
- Map API DTOs to ViewModels

---

# Testing Requirements

All new features must include automated tests.

Required testing layers:

Unit tests
- xUnit
- Moq or NSubstitute

Integration tests
- ASP.NET TestServer
- Testcontainers for infrastructure

Frontend tests
- bUnit

---

# Observability

All services must support observability.

Use:

- OpenTelemetry
- structured logging
- metrics

Recommended stack:

- Prometheus
- Grafana
- Aspire Dashboard

---

# Copilot Guidance

When generating code:

1. Follow the architecture rules in this file.
2. Prefer existing patterns already used in the repository.
3. Reuse domain models, events, and DTOs when possible.
4. Always generate tests for new functionality.
