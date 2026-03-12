
# Architecture Policy

Backend:
- FastEndpoints APIs
- Vertical Slice Architecture
- CQRS when appropriate

Persistence:
EF Core -> write models
Dapper -> read models
MongoDB -> projections
Redis -> caching

Messaging:
RabbitMQ or Azure Service Bus
Outbox pattern required

Testing:
xUnit
bUnit
Testcontainers

Observability:
OpenTelemetry
Prometheus
Grafana
