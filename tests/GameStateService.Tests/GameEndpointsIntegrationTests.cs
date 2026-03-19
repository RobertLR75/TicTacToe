using System.Net;
using System.Net.Http.Json;
using GameStateService.Services;
using GameStateService.Tests.Testing;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using Service.Contracts.Shared;
using Xunit;

namespace GameStateService.Tests;

[Collection(RabbitMqCollection.Name)]
public sealed class GameEndpointsIntegrationTests : IntegrationTestBase
{
    private readonly RabbitMqFixture _fixture;

    public GameEndpointsIntegrationTests(RabbitMqFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Endpoints_get_and_make_move_round_trip_for_initialized_game()
    {
        var persistence = new InMemoryGamePersistenceService();
        var repository = new GameRepository(persistence);
        var initialized = await repository.CreateGameAsync();

        using var factory = new GameStateServiceWebApplicationFactory(_fixture.ConnectionString, repository, persistence, enableEventPublishing: false);
        using var client = factory.CreateClient();

        var getResponse = await client.GetAsync($"/api/game-states/{initialized.GameId}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var game = await getResponse.Content.ReadFromJsonAsync<GetGameStateResponse>();
        Assert.NotNull(game);
        Assert.Equal(initialized.GameId, game!.GameId);

        var moveResponse = await client.PostAsJsonAsync($"/api/game-states/{initialized.GameId}/moves", new UpdateGameStateRequest
        {
            GameId = initialized.GameId,
            Row = 0,
            Col = 0
        });

        Assert.Equal(HttpStatusCode.Accepted, moveResponse.StatusCode);

        var getUpdatedResponse = await client.GetAsync($"/api/game-states/{initialized.GameId}");
        var updated = await getUpdatedResponse.Content.ReadFromJsonAsync<GetGameStateResponse>();

        Assert.NotNull(updated);
        Assert.Equal(PlayerMarkEnum.X, updated!.Board.Single(c => c.Row == 0 && c.Col == 0).Mark);
        Assert.Equal(PlayerMarkEnum.O, updated.CurrentPlayer);
    }
}
