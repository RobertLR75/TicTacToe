using TicTacToeMud.Models;

namespace TicTacToeMud.Services;

public class GameApiClient(HttpClient httpClient)
{
    public virtual async Task<string> CreateGameAsync(string playerId, string playerName)
    {
        var response = await httpClient.PostAsJsonAsync("/api/games", new
        {
            playerId,
            playerName
        });
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateGameApiResponse>();
        return result!.GameId;
    }

    public virtual async Task<IReadOnlyList<GameListItem>> ListGamesAsync()
    {
        var response = await httpClient.GetFromJsonAsync<ListGamesApiResponse>("/api/games");
        return response?.Games ?? [];
    }

    private record CreateGameApiResponse
    {
        public required string GameId { get; init; }
    }

    private record ListGamesApiResponse
    {
        public required List<GameListItem> Games { get; init; }
    }
}
