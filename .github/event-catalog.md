
# Event Catalog

OrderCreatedEvent
Producer: OrdersService
Consumers: InventoryService, PaymentsService

PaymentCapturedEvent
Producer: PaymentsService
Consumers: OrdersService

ShipmentCreatedEvent
Producer: ShippingService
Consumers: NotificationsService
