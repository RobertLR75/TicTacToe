using GameService.Endpoints.Games.List;
using GameService.Models;
using GameService.Persistence;
using GameService.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GameService.Tests;

[Collection(PostgresCollection.Name)]
public sealed class GameStorageServiceIntegrationTests
{
    private readonly PostgresTestContainerFixture _fixture;

    public GameStorageServiceIntegrationTests(PostgresTestContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Create_get_update_and_search_work_against_postgres()
    {
        using var provider = _fixture.BuildServiceProvider();
        await PostgresTestContainerFixture.ResetDatabaseAsync(provider);

        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        var sut = new GameStorageService(db);

        var game = new Game
        {
            Id = Guid.NewGuid(),
            Status = GameStatus.Created,
            Player1 = new Player { Id = "p1", Name = "Alice" }
        };

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
}
