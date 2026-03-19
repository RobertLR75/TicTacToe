using GameService.Features.Games.Endpoints.Get;
using Xunit;

namespace GameService.UnitTests;

public sealed class GetGameHandlerUnitTests : GameServiceUnitTestBase
{
    [Fact]
    public async Task HandleAsync_returns_game_when_store_finds_game()
    {
        var game = CreateGame();
        var sut = CreateGetGameHandler(game);

        var result = await sut.HandleAsync(new GetGameQuery(game.Id));

        Assert.Same(game, result);
    }

    [Fact]
    public async Task HandleAsync_returns_null_when_store_returns_missing_game()
    {
        var sut = CreateGetGameHandler(game: null);

        var result = await sut.HandleAsync(new GetGameQuery(Guid.NewGuid()));

        Assert.Null(result);
    }
}
