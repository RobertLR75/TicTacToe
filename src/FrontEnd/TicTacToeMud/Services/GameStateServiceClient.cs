using TicTacToeMud.Models;

namespace TicTacToeMud.Services;

public class GameStateServiceClient(HttpClient httpClient)
{
    public virtual async Task<GameResponse> GetGameAsync(string gameId)
    {
        return (await httpClient.GetFromJsonAsync<GameResponse>($"/api/game-states/{gameId}"))!;
    }

    public virtual async Task MakeMoveAsync(string gameId, int row, int col)
    {
        var response = await httpClient.PostAsJsonAsync($"/api/game-states/{gameId}/moves", new
        {
            GameId = gameId,
            Row = row,
            Col = col
        });

        response.EnsureSuccessStatusCode();
    }
}
