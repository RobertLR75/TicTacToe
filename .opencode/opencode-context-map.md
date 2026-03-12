# DDD Context Map

This document defines the **bounded contexts and service boundaries**
for the platform. All OpenCode agents must consult this file when
designing systems or generating services.

------------------------------------------------------------------------

# Platform Domains

The system is divided into several **bounded contexts**.

Each bounded context:

-   owns its data
-   exposes APIs
-   publishes integration events
-   does not share databases with other services

------------------------------------------------------------------------

# Core Domains

## Orders Context

Responsibilities:

-   order creation
-   order lifecycle management
-   order status tracking

Key aggregates:

Order\
OrderItem

Domain events:

OrderCreated\
OrderCancelled\
OrderPaid

External integrations:

Inventory\
Payments\
Shipping

------------------------------------------------------------------------

## Inventory Context

Responsibilities:

-   product availability
-   stock tracking
-   warehouse inventory

Aggregates:

ProductInventory\
StockReservation

Events:

StockReserved\
StockReleased\
StockAdjusted

------------------------------------------------------------------------

## Payments Context

Responsibilities:

-   payment authorization
-   payment capture
-   refunds

Aggregates:

Payment\
Transaction

Events:

PaymentAuthorized\
PaymentCaptured\
PaymentFailed

------------------------------------------------------------------------

## Shipping Context

Responsibilities:

-   shipment creation
-   carrier integration
-   delivery tracking

Aggregates:

Shipment\
Package

Events:

ShipmentCreated\
ShipmentDispatched\
ShipmentDelivered

------------------------------------------------------------------------

# Supporting Domains

## Identity Context

Responsibilities:

-   authentication
-   user accounts
-   roles and permissions

------------------------------------------------------------------------

## Notifications Context

Responsibilities:

-   email notifications
-   SMS notifications
-   system alerts

Events consumed:

OrderCreated\
PaymentCaptured\
ShipmentDispatched

------------------------------------------------------------------------

# Context Relationships

Orders → Inventory (reserve stock)\
Orders → Payments (charge payment)\
Orders → Shipping (create shipment)

Inventory publishes stock events consumed by Orders.

Payments publishes payment status events consumed by Orders.

Shipping publishes shipment updates consumed by Orders and
Notifications.

------------------------------------------------------------------------

# Integration Rules

Contexts communicate using **integration events** via messaging.

Supported brokers:

RabbitMQ\
Azure Service Bus

Direct synchronous API calls should be minimized.

------------------------------------------------------------------------

# Data Ownership

Each bounded context must own its database.

Example:

OrdersService → OrdersDB\
InventoryService → InventoryDB\
PaymentsService → PaymentsDB

No cross-service database access is allowed.

------------------------------------------------------------------------

# Frontend Mapping

Blazor UI modules correspond to bounded contexts.

Example:

Orders UI → Orders context\
Inventory UI → Inventory context\
Shipping UI → Shipping context

------------------------------------------------------------------------

# Agent Guidance

When generating new services:

1.  Identify the correct bounded context.
2.  Ensure domain models belong to that context.
3.  Use integration events for cross-context communication.
4.  Avoid coupling between contexts.

------------------------------------------------------------------------

# Goal

Ensure all generated architecture remains:

-   aligned with Domain-Driven Design
-   modular
-   scalable
-   easy to evolve
