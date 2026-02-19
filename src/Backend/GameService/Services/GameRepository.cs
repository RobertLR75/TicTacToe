using GameService.Models;
using System.Collections.Concurrent;

namespace GameService.Services;

public class GameRepository
{
    private readonly ConcurrentDictionary<string, GameState> _games = new();

    public GameState CreateGame()
    {
        var game = new GameState();
        _games[game.GameId] = game;
        return game;
    }

    public GameState? GetGame(string gameId)
    {
        _games.TryGetValue(gameId, out var game);
        return game;
    }

    public void UpdateGame(GameState game)
    {
        _games[game.GameId] = game;
    }

    public void DeleteGame(string gameId)
    {
        _games.TryRemove(gameId, out _);
    }
}

