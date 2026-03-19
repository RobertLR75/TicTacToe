using Service.Contracts.Events;
using SharedLibrary.MassTransit;

namespace GameNotificationService.Features.Notifications.Consumers;


public sealed class GameStateUpdatedConsumer(
    IGameStateUpdatedHandler handler,
    ILogger<GameStateUpdated> logger) : ConsumerBase<GameStateUpdated>(logger)
{
    protected override async Task HandleMessageAsync(GameStateUpdated message, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new GameStateUpdatedCommand(message), cancellationToken);
    }
}