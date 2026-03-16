using GameService.Endpoints.Games.Get;
using GameService.Services;
using NSubstitute;
using Xunit;

namespace GameService.UnitTests;

public sealed class GetGameHandlerUnitTests : GameServiceUnitTestBase
{
    [Fact]
    public async Task HandleAsync_returns_success_when_state_client_finds_game()
    {
        var response = CreateGetGameResponse();
        var client = CreateGameStateReadClient();
        client.GetGameAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(GameStateReadResult.Success(response));

        var sut = new GetGameHandler(client);

        var result = await sut.HandleAsync(new GetGameQuery(Guid.Parse(response.GameId)));

        Assert.True(result.Found);
        Assert.False(result.DependencyUnavailable);
        Assert.Same(response, result.Response);
    }

    [Fact]
    public async Task HandleAsync_returns_not_found_when_state_client_returns_missing_game()
    {
        var client = CreateGameStateReadClient();
        client.GetGameAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(GameStateReadResult.NotFound());

        var sut = new GetGameHandler(client);

        var result = await sut.HandleAsync(new GetGameQuery(Guid.NewGuid()));

        Assert.False(result.Found);
        Assert.False(result.DependencyUnavailable);
        Assert.Null(result.Response);
    }

    [Fact]
    public async Task HandleAsync_returns_unavailable_when_state_client_is_unavailable()
    {
        var client = CreateGameStateReadClient();
        client.GetGameAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(GameStateReadResult.DependencyUnavailable());

        var sut = new GetGameHandler(client);

        var result = await sut.HandleAsync(new GetGameQuery(Guid.NewGuid()));

        Assert.False(result.Found);
        Assert.True(result.DependencyUnavailable);
        Assert.Null(result.Response);
    }
}
