using System.Net;
using System.Net.Http.Json;
using GameStateService.Endpoints.Games.Create;
using GameStateService.Endpoints.Games.Get;
using GameStateService.Endpoints.Games.MakeMove;
using GameStateService.Models;
using GameStateService.Tests.Testing;
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
    public async Task Endpoints_create_get_and_make_move_round_trip()
    {
        using var factory = new GameStateServiceWebApplicationFactory(_fixture.ConnectionString, enableEventPublishing: false);
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsync("/api/games", JsonContent.Create(new { }));

        Assert.Equal(HttpStatusCode.Accepted, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateGameResponse>();
        Assert.NotNull(created);

        var getResponse = await client.GetAsync($"/api/games/{created!.GameId}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var game = await getResponse.Content.ReadFromJsonAsync<GetGameResponse>();
        Assert.NotNull(game);
        Assert.Equal(created.GameId, game!.GameId);

        var moveResponse = await client.PostAsJsonAsync($"/api/games/{created.GameId}/moves", new MakeMoveRequest
        {
            GameId = created.GameId,
            Row = 0,
            Col = 0
        });

        Assert.Equal(HttpStatusCode.Accepted, moveResponse.StatusCode);

        var getUpdatedResponse = await client.GetAsync($"/api/games/{created.GameId}");
        var updated = await getUpdatedResponse.Content.ReadFromJsonAsync<GetGameResponse>();

        Assert.NotNull(updated);
        Assert.Equal(PlayerMark.X, updated!.Board.Single(c => c.Row == 0 && c.Col == 0).Mark);
        Assert.Equal(PlayerMark.O, updated.CurrentPlayer);
    }
}
