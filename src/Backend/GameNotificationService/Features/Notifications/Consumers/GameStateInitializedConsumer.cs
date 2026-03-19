using Service.Contracts.Events;
using SharedLibrary.MassTransit;

namespace GameNotificationService.Features.Notifications.Consumers;

public sealed class GameStateInitializedConsumer(
    IGameStateInitializedHandler handler,
    ILogger<GameStateInitialized> logger) : ConsumerBase<GameStateInitialized>(logger)
{
    protected override async Task HandleMessageAsync(GameStateInitialized message, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new GameStateInitializedCommand(message), cancellationToken);
    }
}