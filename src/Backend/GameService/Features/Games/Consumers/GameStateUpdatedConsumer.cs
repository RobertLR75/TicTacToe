using GameService.Features.Games.Endpoints.UpdateStatus;
using GameService.Features.Games.Entities;
using MassTransit;
using Service.Contracts.Events;

namespace GameService.Features.Games.Consumers;

public sealed class GameStateUpdatedConsumer(
    IUpdateGameStatusHandler handler) : IConsumer<GameStateUpdated>
{
    public async Task Consume(ConsumeContext<GameStateUpdated> context)
    {
        var message = context.Message;

        if (!message.IsOver)
        {
            return;
        }
        
        await handler.HandleAsync(new UpdateGameStatusCommand(message.Id, GameStatus.Completed) , context.CancellationToken);
    }
}
