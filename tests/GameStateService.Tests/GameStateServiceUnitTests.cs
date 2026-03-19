using GameStateService.Configuration;
using GameStateService.Features.GameStates.Entities;
using GameStateService.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Interfaces;
using Xunit;
using GameStateService.Tests.Testing;

namespace GameStateService.Tests;

public sealed class GameStateServiceUnitTests
{
    [Fact]
    public async Task GameRepository_create_get_update_and_delete_round_trip()
    {
        await using var provider = CreateServices();
        var sut = provider.GetRequiredService<IGameRepository>();

        var game = await sut.CreateGameAsync();
        var loaded = await sut.GetGameAsync(game.GameId);

        Assert.NotNull(loaded);
        loaded!.Winner = PlayerMark.X;
        await sut.UpdateGameAsync(loaded);

        var updated = await sut.GetGameAsync(game.GameId);
        Assert.Equal(PlayerMark.X, updated!.Winner);

        await sut.DeleteGameAsync(game.GameId);

        Assert.Null(await sut.GetGameAsync(game.GameId));
    }

    [Fact]
    public async Task GameRepository_create_honors_provided_game_id()
    {
        await using var provider = CreateServices();
        var sut = provider.GetRequiredService<IGameRepository>();
        var requestedGameId = Guid.NewGuid().ToString("D");

        var game = await sut.CreateGameAsync(requestedGameId);

        Assert.Equal(requestedGameId, game.GameId);
        var loaded = await sut.GetGameAsync(requestedGameId);
        Assert.NotNull(loaded);
        Assert.Equal(game.Id, loaded!.Id);
    }

    [Fact]
    public void InitializeGame_command_exposes_requested_game_id()
    {
        var requestedGameId = Guid.NewGuid().ToString("D");

        var command = new GameStateService.Consumers.InitializeGame(requestedGameId);

        Assert.Equal(requestedGameId, command.GameId);
    }

    private static ServiceProvider CreateServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IPersistenceService<GameEntity>, InMemoryGamePersistenceService>();
        services.AddScoped<IGameRepository, GameRepository>();
        return services.BuildServiceProvider();
    }
}
