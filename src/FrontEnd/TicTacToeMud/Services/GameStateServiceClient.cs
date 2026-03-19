using System.Net.Http.Json;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using TicTacToeMud.Models;

namespace TicTacToeMud.Services;

public class GameStateServiceClient(HttpClient httpClient)
{
    public virtual async Task<GameResponse> GetGameAsync(string gameId)
    {
        var response = await httpClient.GetFromJsonAsync<GetGameStateResponse>($"/api/game-states/{gameId}");
        return response!.ToGameResponse();
    }

    public virtual async Task MakeMoveAsync(string gameId, int row, int col)
    {
        var response = await httpClient.PostAsJsonAsync($"/api/game-states/{gameId}/moves", new UpdateGameStateRequest
        {
            GameId = gameId,
            Row = row,
            Col = col
        });

        response.EnsureSuccessStatusCode();
    }
}
