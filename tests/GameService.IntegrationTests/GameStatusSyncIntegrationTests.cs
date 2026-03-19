using GameService.Features.Games.Entities;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Service.Contracts.Events;
using Service.Contracts.Shared;
using Xunit;

namespace GameService.IntegrationTests;

[Collection(PostgresCollection.Name)]
public sealed class GameStatusSyncIntegrationTests : GameServiceIntegrationTestBase
{
    public GameStatusSyncIntegrationTests(PostgresTestContainerFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GameStateUpdated_event_marks_game_completed_when_is_over_is_true()
    {
        await using var factory = CreateFactory();
        await factory.ResetDatabaseAsync();

        var game = await SeedGameAsync(factory.Services, GameStatus.Active);

        await PublishGameStateUpdatedAsync(factory.Services, new GameStateUpdated
        {
            EventId = Guid.NewGuid().ToString("N"),
            SchemaVersion = "1.0",
            Id = game.Id,
            CurrentPlayer = PlayerMarkEnum.O,
            Winner = PlayerMarkEnum.X,
            IsDraw = false,
            IsOver = true,
            Board = [],
            OccurredAtUtc = DateTimeOffset.UtcNow
        });

        var updated = await WaitForStatusAsync(factory.Services, game.Id, GameStatus.Completed);
        Assert.NotNull(updated);
        Assert.Equal(GameStatus.Completed, updated!.Status);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact]
    public async Task GameStateUpdated_event_does_not_change_status_when_game_is_not_over()
    {
        await using var factory = CreateFactory();
        await factory.ResetDatabaseAsync();

        var game = await SeedGameAsync(factory.Services, GameStatus.Active);

        await PublishGameStateUpdatedAsync(factory.Services, new GameStateUpdated
        {
            EventId = Guid.NewGuid().ToString("N"),
            SchemaVersion = "1.0",
            Id = game.Id,
            CurrentPlayer = PlayerMarkEnum.O,
            Winner = PlayerMarkEnum.None,
            IsDraw = false,
            IsOver = false,
            Board = [],
            OccurredAtUtc = DateTimeOffset.UtcNow
        });

        // allow consumer pipeline to process event and settle
        await Task.Delay(250);

        var persisted = await GetGameAsync(factory.Services, game.Id);
        Assert.NotNull(persisted);
        Assert.Equal(GameStatus.Active, persisted!.Status);
    }

    private static async Task PublishGameStateUpdatedAsync(IServiceProvider services, GameStateUpdated @event)
    {
        using var scope = services.CreateScope();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        await publisher.Publish(@event);
    }

    private static async Task<GameEntity?> WaitForStatusAsync(IServiceProvider services, Guid gameId, GameStatus expectedStatus)
    {
        for (var attempt = 0; attempt < 20; attempt++)
        {
            var persisted = await GetGameAsync(services, gameId);
            if (persisted?.Status == expectedStatus)
            {
                return persisted;
            }

            await Task.Delay(100);
        }

        return await GetGameAsync(services, gameId);
    }
}
