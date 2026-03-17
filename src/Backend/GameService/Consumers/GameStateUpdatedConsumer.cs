using GameService.Models;
using GameService.Services;
using MassTransit;
using Service.Contracts.Events;

namespace GameService.Consumers;

public sealed class GameStateUpdatedConsumer(
    IGameStorageService gameStore,
    IGameEventPublisher eventPublisher,
    ILogger<GameStateUpdatedConsumer> logger) : IConsumer<GameStateUpdated>
{
    public async Task Consume(ConsumeContext<GameStateUpdated> context)
    {
        var message = context.Message;

        if (!message.IsOver)
        {
            return;
        }

        if (!Guid.TryParse(message.GameId, out var gameId))
        {
            logger.LogWarning("Received GameStateUpdated with invalid GameId '{GameId}'.", message.GameId);
            return;
        }

        var game = await gameStore.GetAsync(gameId, context.CancellationToken);
        if (game is null)
        {
            logger.LogWarning("Received completion update for unknown game {GameId}.", gameId);
            return;
        }

        if (game.Status == GameStatus.Completed)
        {
            return;
        }

        game.Status = GameStatus.Completed;
        game.UpdatedAt = DateTimeOffset.UtcNow;

        await gameStore.UpdateAsync(game, context.CancellationToken);
        await eventPublisher.PublishEventAsync(GameEventMapper.ToGameStateUpdated(game), context.CancellationToken);
    }
}
