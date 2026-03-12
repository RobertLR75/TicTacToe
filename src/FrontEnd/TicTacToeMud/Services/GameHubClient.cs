using Microsoft.AspNetCore.SignalR.Client;
using TicTacToeMud.Models;

namespace TicTacToeMud.Services;

public class GameHubClient : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private readonly string _hubUrl;

    public event Action<GameResponse>? OnGameStateUpdated;
    public event Action<GameResponse>? OnGameStateInitialized;

    public GameHubClient(IConfiguration configuration)
    {
        _hubUrl = configuration.GetValue<string>("Services:gamenotificationservice:https:0")
                  ?? configuration.GetValue<string>("Services:gamenotificationservice:http:0")
                  ?? "https://localhost:7293";
    }

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public virtual async Task StartAsync(string? hubBaseUrl = null)
    {
        if (_hubConnection is not null)
        {
            await DisposeAsync();
        }

        var targetHubBaseUrl = string.IsNullOrWhiteSpace(hubBaseUrl) ? _hubUrl : hubBaseUrl;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{targetHubBaseUrl}/hubs/game")
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<GameStateUpdatedNotification>("GameStateUpdatedNotification", notification =>
        {
            var response = new GameResponse
            {
                GameId = notification.GameId,
                CurrentPlayer = notification.CurrentPlayer,
                Winner = notification.Winner,
                IsDraw = notification.IsDraw,
                IsOver = notification.IsOver,
                Board = notification.Board
                    .Select(c => new CellDto(c.Row, c.Col, c.Mark))
                    .ToList()
            };

            OnGameStateUpdated?.Invoke(response);
        });

        _hubConnection.On<GameStateInitializedNotification>("GameStateInitializedNotification", notification =>
        {
            var response = new GameResponse
            {
                GameId = notification.GameId,
                CurrentPlayer = notification.CurrentPlayer,
                Winner = notification.Winner,
                IsDraw = notification.IsDraw,
                IsOver = notification.IsOver,
                Board = notification.Board
                    .Select(c => new CellDto(c.Row, c.Col, c.Mark))
                    .ToList()
            };

            OnGameStateInitialized?.Invoke(response);
        });

        await _hubConnection.StartAsync();
    }

    public virtual async Task JoinGame(string gameId)
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.InvokeAsync("JoinGame", gameId);
        }
    }

    public virtual async Task LeaveGame(string gameId)
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.InvokeAsync("LeaveGame", gameId);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
    }
}

public record GameStateUpdatedNotification
{
    public required string GameId { get; init; }
    public required int CurrentPlayer { get; init; }
    public required int Winner { get; init; }
    public required bool IsDraw { get; init; }
    public required bool IsOver { get; init; }
    public required List<CellNotificationDto> Board { get; init; }
}

public record CellNotificationDto(int Row, int Col, int Mark);

public record GameStateInitializedNotification
{
    public required string GameId { get; init; }
    public required int CurrentPlayer { get; init; }
    public required int Winner { get; init; }
    public required bool IsDraw { get; init; }
    public required bool IsOver { get; init; }
    public required List<CellNotificationDto> Board { get; init; }
}
