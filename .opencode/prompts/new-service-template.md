# New Service Template Prompt

Use this prompt when creating a new microservice.

------------------------------------------------------------------------

## Goal

Create a production-ready .NET microservice using:

FastEndpoints\
Vertical Slice Architecture\
CQRS\
Redis caching\
RabbitMQ or Azure Service Bus\
PostgreSQL or MongoDB

The service must integrate with **.NET Aspire AppHost**.

------------------------------------------------------------------------

## Required Outputs

The generated solution must include:

1.  Service architecture overview
2.  Project folder structure
3.  Feature slice example
4.  Messaging integration
5.  Database configuration
6.  Aspire AppHost registration
7.  Unit and integration tests

------------------------------------------------------------------------

## Example Feature Slice

Features/

Orders/ CreateOrder/ Endpoint.cs Request.cs Response.cs Validator.cs
Handler.cs

------------------------------------------------------------------------

## API Rules

Use FastEndpoints.

Endpoints must remain thin.

Business logic must exist in handlers.

------------------------------------------------------------------------

## Data Rules

Use `IStorageService` abstraction.

Select database based on workload:

PostgreSQL → transactional data\
MongoDB → document models\
Redis → caching

------------------------------------------------------------------------

## Messaging

Publish integration events using `IBusService`.

Example:

OrderCreatedEvent

All events must support retry and idempotency.

------------------------------------------------------------------------

## Testing

Generate:

Unit tests for handlers.\
Integration tests using Testcontainers.

Infrastructure containers must include:

PostgreSQL\
Redis\
RabbitMQ

------------------------------------------------------------------------

## Output Format

Architecture\
Project Structure\
Code Examples\
Testing Strategy
