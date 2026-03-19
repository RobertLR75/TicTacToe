using System.Net.Http.Json;
using Service.Contracts.Responses;
using SharedLibrary.HttpClient;

namespace Service.Contracts.Services;

public class GameApiClient(IHttpClientFactory httpClientFactory)
    : BaseApiClient<CreateGamePayload, CreateGameResponse>(httpClientFactory)
{
    protected override string Name => "api/games";

    public virtual async Task<string> CreateGameAsync(string playerId, string playerName, CancellationToken cancellationToken = default)
    {
        var response = await CreateAsync(new CreateGamePayload
        {
            PlayerId = playerId,
            PlayerName = playerName
        }, cancellationToken);

        return response.Id;
    }

    public virtual async Task<IReadOnlyList<GameModel>> ListGamesAsync(CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetFromJsonAsync<ListGamesResponse>($"/{Name}", cancellationToken);
        return response ?? new List<GameModel>();
    }

    public virtual async Task<GetGameResponse> GetGameAsync(string gameId, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetFromJsonAsync<GetGameResponse>($"/{Name}/{gameId}", cancellationToken);
        return response!;
    }
}

public sealed record CreateGamePayload
{
    public required string PlayerId { get; init; }
    public required string PlayerName { get; init; }
}
