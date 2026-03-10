using GameStateService.Configuration;
using GameStateService.Contracts.Events;
using GameStateService.Models;
using GameStateService.Services;
using MassTransit;
using Microsoft.Extensions.Options;
using Xunit;

namespace GameStateService.Tests;

public class EventPublisherIntegrationTests
{
    [Fact]
    public async Task MassTransit_in_memory_wiring_publishes_game_initialized_event()
    {
        var consumed = new TaskCompletionSource<GameStateInitializedEvent>(TaskCreationOptions.RunContinuationsAsynchronously);

        var bus = Bus.Factory.CreateUsingInMemory(cfg =>
        {
            cfg.ReceiveEndpoint("game-initialized-events-test", e =>
            {
                e.Handler<GameStateInitializedEvent>(context =>
                {
                    consumed.TrySetResult(context.Message);
                    return Task.CompletedTask;
                });
            });
        });

        await bus.StartAsync();

        try
        {
            var options = Options.Create(new MessagingOptions { EnableEventPublishing = true });
            var publisher = new MassTransitGameEventPublisher(bus, options);

            var game = new GameState();
            await publisher.PublishEventAsync(GameEventMapper.ToGameStateInitializedEvent(game));

            var completed = await Task.WhenAny(consumed.Task, Task.Delay(TimeSpan.FromSeconds(5)));
            Assert.Same(consumed.Task, completed);
            var message = await consumed.Task;
            Assert.Equal(game.GameId, message.GameId);
        }
        finally
        {
            await bus.StopAsync();
        }
    }
}
