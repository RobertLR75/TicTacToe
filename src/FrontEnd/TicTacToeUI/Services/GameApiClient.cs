using System.Net.Http.Json;
using TicTacToeUI.Models;

namespace TicTacToeUI.Services;

public class GameApiClient
{
    private readonly HttpClient _httpClient;

    public GameApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GameStateDto?> CreateGameAsync()
    {
        var response = await _httpClient.PostAsync("/api/games", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GameStateDto>();
    }

    public async Task<GameStateDto?> GetGameAsync(string gameId)
    {
        return await _httpClient.GetFromJsonAsync<GameStateDto>($"/api/games/{gameId}");
    }

    public async Task<GameStateDto?> MakeMoveAsync(string gameId, int row, int col)
    {
        var request = new { GameId = gameId, Row = row, Col = col };
        var response = await _httpClient.PostAsJsonAsync($"/api/games/{gameId}/moves", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GameStateDto>();
    }
}

public record GameStateDto
{
    public required string GameId { get; init; }
    public required PlayerMark CurrentPlayer { get; init; }
    public required PlayerMark Winner { get; init; }
    public required bool IsDraw { get; init; }
    public required bool IsOver { get; init; }
    public required List<CellDto> Board { get; init; }
}

public record CellDto(int Row, int Col, PlayerMark Mark);

