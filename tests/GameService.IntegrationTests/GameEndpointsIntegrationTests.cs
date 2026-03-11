using System.Net;
using System.Net.Http.Json;
using System.Text;
using GameService.Models;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using Service.Contracts.Shared;
using Xunit;

namespace GameService.IntegrationTests;

[Collection(PostgresCollection.Name)]
public sealed class GameEndpointsIntegrationTests : GameServiceIntegrationTestBase
{
    public GameEndpointsIntegrationTests(PostgresTestContainerFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Create_endpoint_persists_game_and_returns_created_response()
    {
        await using var factory = CreateFactory();
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var request = new CreateGameRequest
        {
            PlayerId = Guid.NewGuid(),
            PlayerName = "Alice"
        };

        var response = await client.PostAsJsonAsync("/api/game-lobby", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<CreateGameResponse>();
        Assert.NotNull(payload);
        Assert.Equal(GameStatusEnum.Created, payload.Status);
        Assert.Equal("Alice", payload.Player1.Name);

        var persisted = await GetGameAsync(factory.Services, payload.Id);
        Assert.NotNull(persisted);
        Assert.Equal(GameStatus.Created, persisted.Status);
        Assert.Equal(request.PlayerId.ToString("D"), persisted.Player1.Id);
        Assert.Equal(request.PlayerName, persisted.Player1.Name);
    }

    [Fact]
    public async Task Create_endpoint_returns_bad_request_for_invalid_payload()
    {
        await using var factory = CreateFactory();
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/game-lobby", new CreateGameRequest
        {
            PlayerId = Guid.Empty,
            PlayerName = string.Empty
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Player id is required", payload);
        Assert.Contains("Player name is required", payload);
    }

    [Fact]
    public async Task Create_endpoint_returns_bad_request_for_player_name_longer_than_50_and_does_not_persist_game()
    {
        await using var factory = CreateFactory();
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/game-lobby", new CreateGameRequest
        {
            PlayerId = Guid.NewGuid(),
            PlayerName = new string('a', 51)
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Player name must be 50 characters or fewer", payload);

        var games = await CreateGamesQuery(factory);
        Assert.Empty(games.Games);
    }

    [Fact]
    public async Task List_endpoint_defaults_to_created_games_only()
    {
        await using var factory = CreateFactory();
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        await SeedGameAsync(factory.Services, GameStatus.Created);
        await SeedGameAsync(factory.Services, GameStatus.Active, "p2", "Bob");

        var response = await client.GetAsync("/api/game-lobby");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ListGamesResponse>();
        Assert.NotNull(payload);
        Assert.Single(payload.Games);
        Assert.All(payload.Games, game => Assert.Equal(GameStatusEnum.Created, game.Status));
    }

    [Fact]
    public async Task List_endpoint_applies_status_filter_and_paging_parameters()
    {
        await using var factory = CreateFactory();
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        await SeedGameAsync(factory.Services, GameStatus.Created);
        await SeedGameAsync(factory.Services, GameStatus.Created, "p2", "Bob");
        await SeedGameAsync(factory.Services, GameStatus.Active, "p3", "Carol");

        var response = await client.GetAsync("/api/game-lobby?status=Created&page=2&pageSize=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ListGamesResponse>();
        Assert.NotNull(payload);
        Assert.Single(payload.Games);
        Assert.Equal(GameStatusEnum.Created, payload.Games[0].Status);
    }

    [Fact]
    public async Task List_endpoint_applies_status_filter_paging_and_created_at_ordering()
    {
        await using var factory = CreateFactory();
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var oldest = await PersistGameAsync(factory.Services, CreateGame(
            id: Guid.Parse("00000000-0000-0000-0000-000000000001"),
            player1: CreatePlayer(),
            createdAt: new DateTimeOffset(2026, 3, 10, 10, 0, 0, TimeSpan.Zero)));
        var middle = await PersistGameAsync(factory.Services, CreateGame(
            id: Guid.Parse("00000000-0000-0000-0000-000000000002"),
            player1: CreatePlayer("p2", "Bob"),
            createdAt: new DateTimeOffset(2026, 3, 10, 10, 5, 0, TimeSpan.Zero)));
        var newest = await PersistGameAsync(factory.Services, CreateGame(
            id: Guid.Parse("00000000-0000-0000-0000-000000000003"),
            player1: CreatePlayer("p3", "Carol"),
            createdAt: new DateTimeOffset(2026, 3, 10, 10, 10, 0, TimeSpan.Zero)));
        await PersistGameAsync(factory.Services, CreateGame(
            id: Guid.Parse("00000000-0000-0000-0000-000000000004"),
            status: GameStatus.Active,
            player1: CreatePlayer("p4", "Dave"),
            createdAt: new DateTimeOffset(2026, 3, 10, 10, 15, 0, TimeSpan.Zero)));

        var page1Response = await client.GetAsync("/api/game-lobby?status=Created&page=1&pageSize=2");
        var page3Response = await client.GetAsync("/api/game-lobby?status=Created&page=3&pageSize=1");

        Assert.Equal(HttpStatusCode.OK, page1Response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, page3Response.StatusCode);

        var page1 = await page1Response.Content.ReadFromJsonAsync<ListGamesResponse>();
        var page3 = await page3Response.Content.ReadFromJsonAsync<ListGamesResponse>();
        Assert.NotNull(page1);
        Assert.NotNull(page3);

        Assert.Equal(new[] { oldest.Id, middle.Id }, page1.Games.Select(x => x.Id).ToArray());
        Assert.Equal(new[] { oldest.CreatedAt, middle.CreatedAt }, page1.Games.Select(x => x.CreatedAt).ToArray());
        Assert.Equal(new[] { newest.Id }, page3.Games.Select(x => x.Id).ToArray());
    }

    [Fact]
    public async Task Update_status_endpoint_updates_status_to_active_and_persists_change()
    {
        await using var factory = CreateFactory();
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/game-lobby", new CreateGameRequest
        {
            PlayerId = Guid.NewGuid(),
            PlayerName = "Alice"
        });

        var created = await createResponse.Content.ReadFromJsonAsync<CreateGameResponse>();
        Assert.NotNull(created);

        var updateResponse = await client.PutAsJsonAsync($"/api/game-lobby/{created.Id}/status", new UpdateGameStatusRequest
        {
            Id = created.Id,
            Status = GameStatusEnum.Active
        });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var payload = await updateResponse.Content.ReadFromJsonAsync<UpdateGameStatusResponse>();
        Assert.NotNull(payload);
        Assert.Equal(created.Id, payload.Id);
        Assert.Equal(GameStatusEnum.Active.ToString(), payload.Status);

        var persisted = await GetGameAsync(factory.Services, created.Id);
        Assert.NotNull(persisted);
        Assert.Equal(GameStatus.Active, persisted.Status);
        Assert.NotNull(persisted.UpdatedAt);
    }

    [Fact]
    public async Task Update_status_endpoint_updates_status_to_completed_and_persists_change()
    {
        await using var factory = CreateFactory();
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var game = await SeedGameAsync(factory.Services, GameStatus.Created);

        var updateResponse = await client.PutAsJsonAsync($"/api/game-lobby/{game.Id}/status", new UpdateGameStatusRequest
        {
            Id = game.Id,
            Status = GameStatusEnum.Completed
        });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var payload = await updateResponse.Content.ReadFromJsonAsync<UpdateGameStatusResponse>();
        Assert.NotNull(payload);
        Assert.Equal(game.Id, payload.Id);
        Assert.Equal(GameStatusEnum.Completed.ToString(), payload.Status);

        var persisted = await GetGameAsync(factory.Services, game.Id);
        Assert.NotNull(persisted);
        Assert.Equal(GameStatus.Completed, persisted.Status);
        Assert.NotNull(persisted.UpdatedAt);
    }

    [Fact]
    public async Task Update_status_endpoint_returns_not_found_for_missing_game()
    {
        await using var factory = CreateFactory();
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync($"/api/game-lobby/{Guid.NewGuid()}/status", new UpdateGameStatusRequest
        {
            Id = Guid.NewGuid(),
            Status = GameStatusEnum.Active
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_status_endpoint_returns_not_found_without_creating_game()
    {
        await using var factory = CreateFactory();
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var missingId = Guid.NewGuid();
        var response = await client.PutAsJsonAsync($"/api/game-lobby/{missingId}/status", new UpdateGameStatusRequest
        {
            Id = missingId,
            Status = GameStatusEnum.Active
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Null(await GetGameAsync(factory.Services, missingId));
    }

    [Fact]
    public async Task Update_status_endpoint_returns_bad_request_for_invalid_status()
    {
        await using var factory = CreateFactory();
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var game = await SeedGameAsync(factory.Services, GameStatus.Created);

        var response = await client.PutAsJsonAsync($"/api/game-lobby/{game.Id}/status", new UpdateGameStatusRequest
        {
            Id = game.Id,
            Status = GameStatusEnum.Created
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Status must be Active or Completed", payload);
    }

    [Fact]
    public async Task Update_status_endpoint_returns_bad_request_when_status_is_missing()
    {
        await using var factory = CreateFactory();
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var game = await SeedGameAsync(factory.Services, GameStatus.Created);
        using var request = new HttpRequestMessage(HttpMethod.Put, $"/api/game-lobby/{game.Id}/status");
        request.Content = new StringContent($"{{\"id\":\"{game.Id}\"}}", Encoding.UTF8, "application/json");

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Status", payload);

        var persisted = await GetGameAsync(factory.Services, game.Id);
        Assert.NotNull(persisted);
        Assert.Equal(GameStatus.Created, persisted.Status);
    }

    [Fact]
    public async Task Update_status_endpoint_returns_bad_request_for_active_to_completed_transition_and_preserves_status()
    {
        await using var factory = CreateFactory();
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var game = await SeedGameAsync(factory.Services, GameStatus.Active);

        var response = await client.PutAsJsonAsync($"/api/game-lobby/{game.Id}/status", new UpdateGameStatusRequest
        {
            Id = game.Id,
            Status = GameStatusEnum.Completed
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Status must be Active or Completed", payload);

        var persisted = await GetGameAsync(factory.Services, game.Id);
        Assert.NotNull(persisted);
        Assert.Equal(GameStatus.Active, persisted.Status);
    }

    [Fact]
    public async Task Update_status_endpoint_allows_active_to_active_and_keeps_status_persisted()
    {
        await using var factory = CreateFactory();
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var game = await SeedGameAsync(factory.Services, GameStatus.Active);

        var response = await client.PutAsJsonAsync($"/api/game-lobby/{game.Id}/status", new UpdateGameStatusRequest
        {
            Id = game.Id,
            Status = GameStatusEnum.Active
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<UpdateGameStatusResponse>();
        Assert.NotNull(payload);
        Assert.Equal(game.Id, payload.Id);
        Assert.Equal(GameStatusEnum.Active.ToString(), payload.Status);

        var persisted = await GetGameAsync(factory.Services, game.Id);
        Assert.NotNull(persisted);
        Assert.Equal(GameStatus.Active, persisted.Status);
    }

    [Fact]
    public async Task Update_status_endpoint_uses_route_id_when_body_id_differs()
    {
        await using var factory = CreateFactory();
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var routeGame = await SeedGameAsync(factory.Services, GameStatus.Created);
        var bodyGame = await SeedGameAsync(factory.Services, GameStatus.Created, "p2", "Bob");

        var response = await client.PutAsJsonAsync($"/api/game-lobby/{routeGame.Id}/status", new UpdateGameStatusRequest
        {
            Id = bodyGame.Id,
            Status = GameStatusEnum.Active
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<UpdateGameStatusResponse>();
        Assert.NotNull(payload);
        Assert.Equal(routeGame.Id, payload.Id);
        Assert.Equal(GameStatusEnum.Active.ToString(), payload.Status);

        var persistedRouteGame = await GetGameAsync(factory.Services, routeGame.Id);
        var persistedBodyGame = await GetGameAsync(factory.Services, bodyGame.Id);
        Assert.NotNull(persistedRouteGame);
        Assert.NotNull(persistedBodyGame);
        Assert.Equal(GameStatus.Active, persistedRouteGame.Status);
        Assert.Equal(GameStatus.Created, persistedBodyGame.Status);
    }

    private static async Task<ListGamesResponse> CreateGamesQuery(GameServiceWebApplicationFactory factory)
    {
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/game-lobby");
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<ListGamesResponse>();
        Assert.NotNull(payload);
        return payload;
    }
}
