# .NET Architecture Policy

These policies define mandatory architecture standards for all generated
services.

## Core Architecture

All backend services must follow:

-   Vertical Slice Architecture
-   FastEndpoints for APIs
-   CQRS separation for commands and queries

Feature slices must contain:

Endpoint\
Request\
Response\
Validator\
Handler\
Mapper

Cross-feature dependencies are forbidden.

------------------------------------------------------------------------

## Persistence Rules

All data access must go through:

`IStorageService`

Supported databases:

PostgreSQL\
SQL Server\
MongoDB\
Redis

Guidelines:

EF Core → write models\
Dapper → read models\
MongoDB → projections / documents\
Redis → caching and ephemeral state

Each microservice must own its database.

------------------------------------------------------------------------

## Messaging Standards

All cross-service communication must be asynchronous.

Supported brokers:

RabbitMQ\
Azure Service Bus

Required reliability patterns:

Outbox Pattern\
Inbox Pattern\
Idempotent consumers\
Dead Letter Queues

Messages must include:

CorrelationId\
CausationId\
TenantId

------------------------------------------------------------------------

## Frontend Standards

Blazor UI must use:

MudBlazor components exclusively.

Rules:

No direct HttpClient calls in UI components.\
Use typed API services.\
Map API DTOs → ViewModels.

State management must use scoped state containers.

------------------------------------------------------------------------

## Testing Requirements

All services must include:

Unit tests\
Integration tests\
Infrastructure tests

Tools:

xUnit\
bUnit\
Testcontainers

Integration tests must run real containers for:

PostgreSQL\
Redis\
MongoDB\
RabbitMQ

------------------------------------------------------------------------

## Observability

All services must export telemetry via:

OpenTelemetry

Required signals:

Logs\
Metrics\
Traces

Recommended stack:

Prometheus\
Grafana\
Aspire Dashboard
