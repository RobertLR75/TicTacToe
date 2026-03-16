using System.Net;
using System.Net.Http.Json;
using Service.Contracts.Responses;

namespace GameService.Services;

public interface IGameStateReadClient
{
    Task<GameStateReadResult> GetGameAsync(Guid gameId, CancellationToken ct = default);
}

public sealed record GameStateReadResult
{
    public required bool Found { get; init; }

    public required bool Unavailable { get; init; }

    public GetGameResponse? Response { get; init; }

    public static GameStateReadResult Success(GetGameResponse response) => new()
    {
        Found = true,
        Unavailable = false,
        Response = response
    };

    public static GameStateReadResult NotFound() => new()
    {
        Found = false,
        Unavailable = false
    };

    public static GameStateReadResult DependencyUnavailable() => new()
    {
        Found = false,
        Unavailable = true
    };
}

public sealed class GameStateReadClient(HttpClient httpClient) : IGameStateReadClient
{
    public async Task<GameStateReadResult> GetGameAsync(Guid gameId, CancellationToken ct = default)
    {
        try
        {
            using var response = await httpClient.GetAsync($"/api/game-states/{gameId}", ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return GameStateReadResult.NotFound();
            }

            if (!response.IsSuccessStatusCode)
            {
                return GameStateReadResult.DependencyUnavailable();
            }

            var payload = await response.Content.ReadFromJsonAsync<GetGameResponse>(cancellationToken: ct);
            return payload is null
                ? GameStateReadResult.DependencyUnavailable()
                : GameStateReadResult.Success(payload);
        }
        catch (HttpRequestException)
        {
            return GameStateReadResult.DependencyUnavailable();
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            return GameStateReadResult.DependencyUnavailable();
        }
    }
}
