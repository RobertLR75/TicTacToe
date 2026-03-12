using System.Net;
using System.Net.Http.Json;
using System.Text;
using TicTacToeMud.Services;
using Xunit;

namespace TicTacToeMud.Tests;

public class GameStateServiceClientTests
{
    [Fact]
    public async Task GetGameAsync_sends_get_to_game_state_endpoint()
    {
        HttpRequestMessage? capturedRequest = null;

        using var client = CreateHttpClient(async request =>
        {
            capturedRequest = request;
            await Task.CompletedTask;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"gameId\":\"game-123\",\"currentPlayer\":1,\"winner\":0,\"isDraw\":false,\"isOver\":false,\"board\":[]}",
                    Encoding.UTF8,
                    "application/json")
            };
        });

        var sut = new GameStateServiceClient(client);

        var game = await sut.GetGameAsync("game-123");

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Get, capturedRequest!.Method);
        Assert.Equal("https://example.test/api/game-states/game-123", capturedRequest.RequestUri!.ToString());
        Assert.Equal("game-123", game.GameId);
    }

    [Fact]
    public async Task MakeMoveAsync_sends_post_to_game_state_moves_endpoint()
    {
        HttpRequestMessage? capturedRequest = null;

        using var client = CreateHttpClient(async request =>
        {
            capturedRequest = request;
            await Task.CompletedTask;

            return new HttpResponseMessage(HttpStatusCode.Accepted);
        });

        var sut = new GameStateServiceClient(client);

        await sut.MakeMoveAsync("game-123", 1, 2);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest!.Method);
        Assert.Equal("https://example.test/api/game-states/game-123/moves", capturedRequest.RequestUri!.ToString());

        var payload = await capturedRequest.Content!.ReadFromJsonAsync<MakeMovePayload>();
        Assert.Equal("game-123", payload!.GameId);
        Assert.Equal(1, payload.Row);
        Assert.Equal(2, payload.Col);
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

    private sealed record MakeMovePayload(string GameId, int Row, int Col);
}
