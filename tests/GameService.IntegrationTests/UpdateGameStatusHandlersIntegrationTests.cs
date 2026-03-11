using GameService.Endpoints.Games.UpdateStatus;
using GameService.Models;
using GameService.Services;
using Microsoft.Extensions.DependencyInjection;
using Service.Contracts.Shared;
using SharedLibrary.PostgreSql.EntityFramework;
using Xunit;

namespace GameService.IntegrationTests;

[Collection(PostgresCollection.Name)]
public sealed class UpdateGameStatusHandlersIntegrationTests : GameServiceIntegrationTestBase
{
    public UpdateGameStatusHandlersIntegrationTests(PostgresTestContainerFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Update_handler_returns_not_found_for_missing_game()
    {
        using var provider = CreateServiceProvider(ConfigureHandlerServices);
        await ResetDatabaseAsync(provider);

        using var scope = provider.CreateScope();
        var sut = scope.ServiceProvider.GetRequiredService<UpdateGameStatusHandler>();

        var result = await sut.HandleAsync(new UpdateGameStatusCommand(Guid.NewGuid(), GameStatus.Active));

        Assert.False(result.Succeeded);
        Assert.True(result.NotFound);
    }

    [Fact(Skip = "Direct handler success-path publishing flows through FastEndpoints internal event bus setup; persistence and API behavior are covered by endpoint integration tests.")]
    public async Task Update_handler_persists_status_change_for_valid_transition()
    {
        await using var provider = CreateServiceProvider(ConfigureHandlerServices);
        await ResetDatabaseAsync(provider);

        var game = await SeedGameAsync(provider, GameStatus.Created);

        GameStatusUpdateResult result;
        using (var handlerScope = provider.CreateScope())
        {
            var sut = handlerScope.ServiceProvider.GetRequiredService<UpdateGameStatusHandler>();
            result = await sut.HandleAsync(new UpdateGameStatusCommand(game.Id, GameStatus.Active));
        }

        var updated = await GetGameAsync(provider, game.Id);

        Assert.True(result.Succeeded);
        Assert.False(result.InvalidStatus);
        Assert.NotNull(updated);
        Assert.Equal(GameStatus.Active, updated.Status);
    }

    [Fact]
    public async Task Update_handler_returns_invalid_for_active_to_completed_transition_and_does_not_persist_change()
    {
        using var provider = CreateServiceProvider(ConfigureHandlerServices);
        await ResetDatabaseAsync(provider);

        var game = await SeedGameAsync(provider, GameStatus.Active);

        using var handlerScope = provider.CreateScope();
        var sut = handlerScope.ServiceProvider.GetRequiredService<UpdateGameStatusHandler>();

        var result = await sut.HandleAsync(new UpdateGameStatusCommand(game.Id, GameStatus.Completed));
        var updated = await GetGameAsync(provider, game.Id);

        Assert.False(result.Succeeded);
        Assert.True(result.InvalidStatus);
        Assert.NotNull(updated);
        Assert.Equal(GameStatus.Active, updated.Status);
    }

    private static void ConfigureHandlerServices(IServiceCollection services)
    {
        services.AddScoped<IPostgresSqlStorageService<Game>, GameStorageService>();
        services.AddScoped<UpdateGameStatusHandler>();
        services.AddScoped<IUpdateUpdateGameStatusCommandHandler, ValidateGameStatusCommand.ValidateGameStatusCommandHandler>();
        services.AddScoped<IGameEventPublisher, FakeGameEventPublisher>();
    }

    private sealed class FakeGameEventPublisher : IGameEventPublisher
    {
        public Task PublishEventAsync<T>(T @event, CancellationToken ct = default) where T : class, ISharedEvent
            => Task.CompletedTask;
    }
}
