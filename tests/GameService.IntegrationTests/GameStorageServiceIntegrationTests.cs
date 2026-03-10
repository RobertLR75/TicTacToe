using GameService.Endpoints.Games.List;
using GameService.Models;
using GameService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GameService.IntegrationTests;

[Collection(PostgresCollection.Name)]
public sealed class GameStorageServiceIntegrationTests : GameServiceIntegrationTestBase
{
    public GameStorageServiceIntegrationTests(PostgresTestContainerFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Create_get_update_and_search_work_against_postgres()
    {
        await using var provider = CreateServiceProvider();
        await ResetDatabaseAsync(provider);

        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DbContext>();
        var sut = new GameStorageService(db);

        var game = CreateGame();

        await sut.CreateAsync(game);

        game.Status = GameStatus.Active;
        game.UpdatedAt = DateTimeOffset.UtcNow;
        await sut.UpdateAsync(game);

        var loaded = await sut.GetAsync(game.Id);
        var search = await sut.SearchAsync(new SearchByStatusSpecification(GameStatus.Active, page: 1, pageSize: 10));

        Assert.NotNull(loaded);
        Assert.Equal(GameStatus.Active, loaded!.Status);
        Assert.Contains(search, g => g.Id == game.Id);
    }

    [Fact]
    public async Task GetAsync_returns_null_for_missing_game()
    {
        await using var provider = CreateServiceProvider();
        await ResetDatabaseAsync(provider);

        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DbContext>();
        var sut = new GameStorageService(db);

        var loaded = await sut.GetAsync(Guid.NewGuid());

        Assert.Null(loaded);
    }

    [Fact]
    public async Task SearchAsync_returns_empty_when_no_games_match_status()
    {
        await using var provider = CreateServiceProvider();
        await ResetDatabaseAsync(provider);

        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DbContext>();
        var sut = new GameStorageService(db);

        await sut.CreateAsync(CreateGame(GameStatus.Created));

        var search = await sut.SearchAsync(new SearchByStatusSpecification(GameStatus.Completed, page: 1, pageSize: 10));

        Assert.Empty(search);
    }
}
