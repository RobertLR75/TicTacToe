# Game Notification Event Consumers

## Queue Endpoints

- `gamenotificationservice-game-created`: consumes `GameCreatedEvent`
- `gamenotificationservice-game-state-updated`: consumes `GameStateUpdatedEvent`

## Required Configuration

Set these values under `Messaging`:

- `EnableEventConsumers`: set to `true` to enable RabbitMQ consumers
- `RabbitMq:Host`: broker host name
- `RabbitMq:Port`: broker port (default `5672`)
- `RabbitMq:VirtualHost`: virtual host path (default `/`)
- `RabbitMq:Username`: broker username
- `RabbitMq:Password`: broker password

You can also provide `ConnectionStrings:rabbitmq`. When `EnableEventConsumers=true`, the service applies host, port, virtual host, and credentials from that URI.

## Local Run

1. Start infrastructure with Aspire (`TicTacToe.AppHost`) so RabbitMQ is available.
2. Ensure `Messaging:EnableEventConsumers=true` for `GameNotificationService`.
3. Start `GameNotificationService`.
4. Publish `GameCreatedEvent` or `GameStateUpdatedEvent` from `GameStateService`.
5. Verify console output in `GameNotificationService` for received events.

## Rollback

1. Set `Messaging:EnableEventConsumers=false` for `GameNotificationService`.
2. Redeploy or restart service.
3. Confirm consumers are no longer receiving events and API behavior is unchanged.

If immediate rollback is required, deploy the previous service version while keeping the same HTTP endpoint contract.
