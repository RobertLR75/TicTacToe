using GameService.Models;
using GameService.Persistence;
using GameService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.PostgreSql.EntityFramework;
using Xunit;

namespace GameService.Tests;

[Collection(PostgresCollection.Name)]
public sealed class UpdateGameStatusHandlersIntegrationTests
{
    private readonly PostgresTestContainerFixture _fixture;

    public UpdateGameStatusHandlersIntegrationTests(PostgresTestContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Update_handler_returns_not_found_for_missing_game()
    {
        using var provider = BuildServiceProvider();
        await PostgresTestContainerFixture.ResetDatabaseAsync(provider);

        using var scope = provider.CreateScope();
        var sut = scope.ServiceProvider.GetRequiredService<IUpdateGameStatusCommandHandler>();

        var result = await sut.HandleAsync(new UpdateGameStatusCommand(Guid.NewGuid(), GameStatus.Active));

        Assert.False(result.Succeeded);
        Assert.True(result.NotFound);
    }

    [Fact]
    public async Task Update_handler_persists_status_change_for_valid_transition()
    {
        using var provider = BuildServiceProvider();
        await PostgresTestContainerFixture.ResetDatabaseAsync(provider);

        var game = new GameModel
        {
            Id = Guid.NewGuid(),
            Status = GameStatus.Created,
            Player1 = new PlayerModel { Id = "p1", Name = "Alice" }
        };

        using (var seedScope = provider.CreateScope())
        {
            var seedStore = seedScope.ServiceProvider.GetRequiredService<IPostgresSqlStorageService<GameModel>>();
            await seedStore.CreateAsync(game);
        }

        GameStatusUpdateResult result;
        using (var handlerScope = provider.CreateScope())
        {
            var sut = handlerScope.ServiceProvider.GetRequiredService<IUpdateGameStatusCommandHandler>();
            result = await sut.HandleAsync(new UpdateGameStatusCommand(game.Id, GameStatus.Active));
        }

        GameModel? updated;
        using (var readScope = provider.CreateScope())
        {
            var readStore = readScope.ServiceProvider.GetRequiredService<IPostgresSqlStorageService<GameModel>>();
            updated = await readStore.GetAsync(game.Id);
        }

        Assert.True(result.Succeeded);
        Assert.False(result.InvalidStatus);
        Assert.NotNull(updated);
        Assert.Equal(GameStatus.Active, updated!.Status);
    }

    private ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:postgres"] = _fixture.ConnectionString
            })
            .Build();

        services.AddGamePersistence(config);
        services.AddScoped<IPostgresSqlStorageService<GameModel>, GameStorageService>();
        services.AddScoped<IUpdateGameStatusCommandHandler, UpdateGameStatusCommand.UpdateUpdateGameStatusCommandHandler>();
        services.AddScoped<IUpdateUpdateGameStatusCommandHandler, ValidateGameStatusCommand.ValidateGameStatusCommandHandler>();
        return services.BuildServiceProvider();
    }
}
