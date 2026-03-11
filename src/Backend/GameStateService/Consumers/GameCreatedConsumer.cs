using GameStateService.Services;
using MassTransit;
using Service.Contracts.Events;

namespace GameStateService.Consumers;

public class GameCreatedConsumer : IConsumer<GameCreated>
{
    private readonly IRequestHandler<InitializeGame, Models.GameState> _initializeGameHandler;
    private readonly ILogger<GameCreatedConsumer> _logger;

    public GameCreatedConsumer(
        IRequestHandler<InitializeGame, Models.GameState> initializeGameHandler,
        ILogger<GameCreatedConsumer> logger)
    {
        _initializeGameHandler = initializeGameHandler;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GameCreated> context)
    {
        _logger.LogInformation("Received GameCreatedEvent for GameId: {GameId}", context.Message.GameId);
        await _initializeGameHandler.HandleAsync(new InitializeGame(), context.CancellationToken);
    }
}
