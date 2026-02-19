using TicTacToeUI.Models;

namespace TicTacToeUI.Services;

public class GameService
{
    private readonly GameApiClient _apiClient;
    public GameState CurrentState { get; private set; } = new();

    public GameService(GameApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task StartNewGameAsync()
    {
        var dto = await _apiClient.CreateGameAsync();
        if (dto is not null)
        {
            UpdateStateFromDto(dto);
        }
    }

    public async Task MakeMoveAsync(int row, int col)
    {
        if (CurrentState.IsOver) return;
        if (!CurrentState.Board.IsEmpty(row, col)) return;

        var dto = await _apiClient.MakeMoveAsync(CurrentState.GameId, row, col);
        if (dto is not null)
        {
            UpdateStateFromDto(dto);
        }
    }

    private void UpdateStateFromDto(GameStateDto dto)
    {
        CurrentState.GameId = dto.GameId;
        CurrentState.CurrentPlayer = dto.CurrentPlayer;
        CurrentState.Winner = dto.Winner;
        CurrentState.IsDraw = dto.IsDraw;

        // Update board
        foreach (var cellDto in dto.Board)
        {
            CurrentState.Board.SetCell(cellDto.Row, cellDto.Col, cellDto.Mark);
        }
    }
}
