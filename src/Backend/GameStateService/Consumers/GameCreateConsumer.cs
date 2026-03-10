using MassTransit;
using Service.Contracts.CreateGame;

namespace GameStateService.Consumers;

public class GameCreateConsumer : IConsumer<GameCreated>
{
    private readonly ILogger<GameCreateConsumer> _logger;

    public GameCreateConsumer(ILogger<GameCreateConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GameCreated> context)
    {
        _logger.LogInformation("Received GameCreatedEvent for GameId: {GameId}", context.Message.GameId);
        // TODO: Implement logic to initialize game state if not already handled by command handler
        await Task.CompletedTask;
    }
}

