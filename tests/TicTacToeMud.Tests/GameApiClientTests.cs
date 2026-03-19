using System.Net;
using System.Net.Http.Json;
using System.Text;
using Service.Contracts.Responses;
using Service.Contracts.Shared;
using Service.Contracts.Services;
using Xunit;

namespace TicTacToeMud.Tests;

public class GameApiClientTests
{
    [Fact]
    public async Task CreateGameAsync_sends_identity_payload_and_returns_game_id()
    {
        HttpRequestMessage? capturedRequest = null;

        using var client = CreateHttpClient(async request =>
        {
            capturedRequest = request;
            await Task.CompletedTask;

            return new HttpResponseMessage(HttpStatusCode.Accepted)
            {
                Content = new StringContent(
                    "{\"id\":\"2a08ef26-61f1-4304-8db3-9b43db8ad547\",\"status\":0,\"player1\":{\"playerId\":\"player-1\",\"name\":\"Alice\"}}",
                    Encoding.UTF8,
                    "application/json")
            };
        });

        var sut = new GameApiClient(new FixedHttpClientFactory(client));

        var gameId = await sut.CreateGameAsync("player-1", "Alice");

        Assert.Equal("2a08ef26-61f1-4304-8db3-9b43db8ad547", gameId);
        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest!.Method);
        Assert.Equal("https://example.test/api/games", capturedRequest.RequestUri!.ToString());

        var payload = await capturedRequest.Content!.ReadFromJsonAsync<CreateGamePayload>();
        Assert.Equal("player-1", payload!.PlayerId);
        Assert.Equal("Alice", payload.PlayerName);
    }

    [Fact]
    public async Task ListGamesAsync_sends_get_and_maps_response()
    {
        HttpRequestMessage? capturedRequest = null;

        using var client = CreateHttpClient(async request =>
        {
            capturedRequest = request;
            await Task.CompletedTask;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "[{\"gameId\":\"2a08ef26-61f1-4304-8db3-9b43db8ad547\",\"status\":0,\"createdAt\":\"2026-01-01T10:00:00Z\",\"updatedAt\":null,\"player1\":{\"playerId\":\"player-1\",\"name\":\"Alice\"},\"player2\":null}]",
                    Encoding.UTF8,
                    "application/json")
            };
        });

        var sut = new GameApiClient(new FixedHttpClientFactory(client));

        var games = await sut.ListGamesAsync();

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Get, capturedRequest!.Method);
        Assert.Equal("https://example.test/api/games", capturedRequest.RequestUri!.ToString());
        Assert.Single(games);
        Assert.Equal("2a08ef26-61f1-4304-8db3-9b43db8ad547", games[0].GameId);
        Assert.Equal(GameStatusEnum.Created, games[0].Status);
        Assert.Equal("Alice", games[0].Player1.Name);
        Assert.Equal("player-1", games[0].Player1.PlayerId);
        Assert.Null(games[0].Player2);
    }

    [Fact]
    public async Task GetGameAsync_sends_get_to_gameservice_route_and_maps_response()
    {
        HttpRequestMessage? capturedRequest = null;

        using var client = CreateHttpClient(async request =>
        {
            capturedRequest = request;
            await Task.CompletedTask;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"gameId\":\"game-123\",\"status\":0,\"createdAt\":\"2026-01-01T10:00:00Z\",\"updatedAt\":null,\"player1\":{\"playerId\":\"player-1\",\"name\":\"Alice\"},\"player2\":null}",
                    Encoding.UTF8,
                    "application/json")
            };
        });

        var sut = new GameApiClient(new FixedHttpClientFactory(client));

        var game = await sut.GetGameAsync("game-123");

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Get, capturedRequest!.Method);
        Assert.Equal("https://example.test/api/games/game-123", capturedRequest.RequestUri!.ToString());
        Assert.Equal("game-123", game.GameId);
        Assert.Equal(GameStatusEnum.Created, game.Status);
        Assert.Equal("player-1", game.Player1.PlayerId);
        Assert.Equal("Alice", game.Player1.Name);
    }

    private static HttpClient CreateHttpClient(Func<HttpRequestMessage, Task<HttpResponseMessage>> responder)
    {
        return new HttpClient(new StubHttpMessageHandler(responder))
        {
            BaseAddress = new Uri("https://example.test")
        };
    }

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> responder) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return responder(request);
        }
    }

    private sealed class FixedHttpClientFactory(HttpClient client) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => client;
    }
}
