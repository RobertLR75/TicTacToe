using TicTacToeMud.Models;

namespace TicTacToeMud.Services;

public class GameApiClient(HttpClient httpClient)
{
    public async Task<string> CreateGameAsync()
    {
        var response = await httpClient.PostAsync("/api/games", null);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateGameApiResponse>();
        return result!.GameId;
    }

    public async Task<GameResponse> GetGameAsync(string gameId)
    {
        return (await httpClient.GetFromJsonAsync<GameResponse>($"/api/games/{gameId}"))!;
    }

    public async Task MakeMoveAsync(string gameId, int row, int col)
    {
        var response = await httpClient.PostAsJsonAsync($"/api/games/{gameId}/moves", new
        {
            GameId = gameId,
            Row = row,
            Col = col
        });
        response.EnsureSuccessStatusCode();
    }

    private record CreateGameApiResponse
    {
        public required string GameId { get; init; }
    }
}
