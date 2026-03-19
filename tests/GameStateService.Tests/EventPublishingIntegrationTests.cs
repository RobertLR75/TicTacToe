using GameStateService.Features.GameStates.Endpoints.Update;
using GameStateService.Services;
using GameStateService.Tests.Testing;
using Xunit;

namespace GameStateService.Tests;

public sealed class EventPublishingIntegrationTests
{
    [Fact]
    public async Task UpdateGameStateHandler_returns_not_found_when_game_is_missing()
    {
        var repository = new GameRepository(new InMemoryGamePersistenceService());
        var sut = new UpdateGameStateHandler(repository, new GameStateHandler(new CheckWinnerHandler(), new CheckDrawHandler()));

        var result = await sut.HandleAsync(new UpdateGameStateCommand("missing", 0, 0));

        Assert.Equal(MakeMoveCommandStatus.NotFound, result.Status);
    }
}
