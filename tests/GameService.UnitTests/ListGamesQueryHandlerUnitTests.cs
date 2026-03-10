using GameService.Endpoints.Games.List;
using GameService.Models;
using NSubstitute;
using SharedLibrary.PostgreSql.EntityFramework;
using Xunit;

namespace GameService.UnitTests;

public class ListGamesQueryHandlerUnitTests : GameServiceUnitTestBase
{
    [Fact]
    public async Task HandleAsync_queries_store_with_search_specification_and_returns_games()
    {
        var expected = new List<Game>
        {
            CreateGame(GameStatus.Created)
        };

        var store = CreateStore();
        store.SearchAsync(Arg.Any<SearchByStatusSpecification>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expected));

        var sut = new ListGamesQueryHandler(store);

        var result = await sut.HandleAsync(new ListGamesQuery(GameStatus.Created, 2, 5));

        Assert.Same(expected, result);
        await store.Received(1).SearchAsync(Arg.Any<SearchByStatusSpecification>(), Arg.Any<CancellationToken>());
    }
}
