# Integration Event Catalog

This document defines all **integration events** used across the
platform. All OpenCode agents must reference this file when publishing
or consuming events.

Goals:

-   prevent inconsistent event naming
-   standardize event payloads
-   document producers and consumers
-   enforce event-driven architecture rules

------------------------------------------------------------------------

# Event Naming Convention

Events must follow:

`<Aggregate>`{=html}`<PastTenseAction>`{=html}Event

Examples:

OrderCreatedEvent\
PaymentCapturedEvent\
ShipmentDispatchedEvent

Rules:

-   Events represent facts that already happened
-   Events must be immutable
-   Events must be versionable

------------------------------------------------------------------------

# Event Envelope

All events must include metadata.

Standard envelope:

EventId\
CorrelationId\
CausationId\
Timestamp\
TenantId

Payload contains the domain data.

------------------------------------------------------------------------

# Orders Context Events

## OrderCreatedEvent

Description: Raised when a new order is successfully created.

Producer: OrdersService

Consumers: InventoryService\
PaymentsService\
NotificationsService

Payload:

OrderId\
CustomerId\
TotalAmount\
Currency\
Items

------------------------------------------------------------------------

## OrderCancelledEvent

Description: Raised when an order is cancelled.

Producer: OrdersService

Consumers: InventoryService\
PaymentsService\
NotificationsService

Payload:

OrderId\
Reason

------------------------------------------------------------------------

# Inventory Context Events

## StockReservedEvent

Description: Inventory reserved for an order.

Producer: InventoryService

Consumers: OrdersService

Payload:

OrderId\
ProductId\
Quantity

------------------------------------------------------------------------

## StockReleasedEvent

Description: Inventory released due to cancellation.

Producer: InventoryService

Consumers: OrdersService

Payload:

OrderId\
ProductId\
Quantity

------------------------------------------------------------------------

# Payments Context Events

## PaymentAuthorizedEvent

Description: Payment authorization succeeded.

Producer: PaymentsService

Consumers: OrdersService

Payload:

OrderId\
PaymentId\
Amount\
Currency

------------------------------------------------------------------------

## PaymentFailedEvent

Description: Payment authorization failed.

Producer: PaymentsService

Consumers: OrdersService\
NotificationsService

Payload:

OrderId\
PaymentId\
FailureReason

------------------------------------------------------------------------

# Shipping Context Events

## ShipmentCreatedEvent

Description: Shipment created for an order.

Producer: ShippingService

Consumers: NotificationsService

Payload:

OrderId\
ShipmentId\
Carrier

------------------------------------------------------------------------

## ShipmentDeliveredEvent

Description: Shipment successfully delivered.

Producer: ShippingService

Consumers: OrdersService\
NotificationsService

Payload:

OrderId\
ShipmentId\
DeliveredAt

------------------------------------------------------------------------

# Event Publishing Rules

Events must be published using:

IBusService

Direct broker SDK usage is forbidden.

------------------------------------------------------------------------

# Reliability Rules

All event publishing must support:

Outbox Pattern\
Retry Policies\
Dead Letter Queues\
Idempotent Consumers

------------------------------------------------------------------------

# Versioning Strategy

Events must support versioning.

Example:

OrderCreatedEventV2

Guidelines:

-   Add fields without breaking consumers
-   Never remove existing fields
-   Deprecate older versions gradually

------------------------------------------------------------------------

# Agent Guidance

When generating new services:

1.  Check if a relevant event already exists.
2.  Reuse existing events where possible.
3.  Only introduce new events if necessary.
4.  Update this catalog when new events are created.

------------------------------------------------------------------------

# Goal

Ensure consistent event-driven communication across the entire platform.
