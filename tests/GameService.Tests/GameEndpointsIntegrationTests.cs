using System.Net;
using System.Net.Http.Json;
using GameService.Contracts;
using GameService.Endpoints.Games.List;
using Xunit;

namespace GameService.Tests;

[Collection(PostgresCollection.Name)]
public sealed class GameEndpointsIntegrationTests
{
    private readonly PostgresTestContainerFixture _fixture;

    public GameEndpointsIntegrationTests(PostgresTestContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Create_endpoint_persists_game_and_returns_created_response()
    {
        using var factory = new GameServiceWebApplicationFactory(_fixture.ConnectionString);
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/game-lobby", new CreateGameRequest
        {
            PlayerId = Guid.NewGuid(),
            PlayerName = "Alice"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<CreateGameResponse>();
        Assert.NotNull(payload);
        Assert.Equal(GameStatusEnum.Created, payload!.Status);
        Assert.Equal("Alice", payload.Player1.Name);
    }

    [Fact]
    public async Task List_endpoint_returns_created_games()
    {
        using var factory = new GameServiceWebApplicationFactory(_fixture.ConnectionString);
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        await client.PostAsJsonAsync("/api/game-lobby", new CreateGameRequest
        {
            PlayerId = Guid.NewGuid(),
            PlayerName = "Alice"
        });

        var response = await client.GetAsync("/api/game-lobby?status=Created&page=1&pageSize=20");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ListGamesResponse>();
        Assert.NotNull(payload);
        Assert.NotEmpty(payload!.Games);
        Assert.All(payload.Games, g => Assert.Equal(GameService.Models.GameStatus.Created, g.Status));
    }

    [Fact]
    public async Task Update_status_endpoint_updates_status_to_active()
    {
        using var factory = new GameServiceWebApplicationFactory(_fixture.ConnectionString);
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/game-lobby", new CreateGameRequest
        {
            PlayerId = Guid.NewGuid(),
            PlayerName = "Alice"
        });

        var created = await createResponse.Content.ReadFromJsonAsync<CreateGameResponse>();
        Assert.NotNull(created);

        var updateResponse = await client.PutAsJsonAsync($"/api/game-lobby/{created!.Id}/status", new UpdateGameStatusRequest
        {
            Id = created.Id,
            Status = GameStatusEnum.Active
        });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var payload = await updateResponse.Content.ReadFromJsonAsync<UpdateGameStatusResponse>();
        Assert.NotNull(payload);
        Assert.Equal(created.Id, payload!.Id);
        Assert.Equal(GameStatusEnum.Active.ToString(), payload.Status);
    }
}
