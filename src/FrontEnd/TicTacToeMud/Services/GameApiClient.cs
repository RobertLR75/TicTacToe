using Service.Contracts.Responses;
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
        var result = await response.Content.ReadFromJsonAsync<CreateGameResponse>();
        return result!.Id.ToString();
    }

    public virtual async Task<IReadOnlyList<GameListItem>> ListGamesAsync()
    {
        var response = await httpClient.GetFromJsonAsync<ListGamesResponse>("/api/games");
        return response?.Games.Select(game => game.ToGameListItem()).ToList() ?? [];
    }

    
   
}
