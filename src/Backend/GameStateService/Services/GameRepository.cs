using System.Collections.Concurrent;

namespace GameStateService.Services;

public class GameRepository : IGameRepository
{
    private readonly ConcurrentDictionary<string, Models.GameState> _games = new();

    public Models.GameState CreateGame()
    {
        var game = new Models.GameState();
        _games[game.GameId] = game;
        return game;
    }

    public Models.GameState? GetGame(string gameId)
    {
        _games.TryGetValue(gameId, out var game);
        return game;
    }

    public void UpdateGame(Models.GameState game)
    {
        _games[game.GameId] = game;
    }

    public void DeleteGame(string gameId)
    {
        _games.TryRemove(gameId, out _);
    }
}
