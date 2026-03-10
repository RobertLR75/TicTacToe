namespace GameStateService.Services;

public interface IGameRepository
{
    Models.GameState CreateGame();

    Models.GameState? GetGame(string gameId);

    void UpdateGame(Models.GameState game);

    void DeleteGame(string gameId);
}
