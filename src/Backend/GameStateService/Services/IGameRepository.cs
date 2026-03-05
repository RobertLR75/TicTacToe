using GameStateService.Models;

namespace GameStateService.Services;

public interface IGameRepository
{
    GameState CreateGame();

    GameState? GetGame(string gameId);

    void UpdateGame(GameState game);

    void DeleteGame(string gameId);
}
