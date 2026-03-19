using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Service.Contracts.Models;
using Service.Contracts.Responses;
using SharedLibrary.HubClient;

namespace Service.Contracts.Services;

public class GameHubClient : BaseHubClient
{
    public event Action<GameHubModel>? OnGameStateUpdated;
    public event Action<GameHubModel>? OnGameStateInitialized;

    protected override async Task<Task> InitializeHubAsync()
    {
        if (Connection is null)
            return Task.CompletedTask;
        
        Connection.On<GameStateUpdatedNotification>("GameStateUpdatedNotification", notification =>
        {
            var response = new GameHubModel
            {
                GameId = notification.GameId,
                CurrentPlayer = notification.CurrentPlayer,
                Winner = notification.Winner,
                IsDraw = notification.IsDraw,
                IsOver = notification.IsOver,
                Board = notification.Board
                    .Select(c => new GameHubCell(c.Row, c.Col, c.Mark))
                    .ToList()
            };

            OnGameStateUpdated?.Invoke(response);
        });

        Connection.On<GameStateInitializedNotification>("GameStateInitializedNotification", notification =>
        {
            var response = new GameHubModel
            {
                GameId = notification.GameId,
                CurrentPlayer = notification.CurrentPlayer,
                Winner = notification.Winner,
                IsDraw = notification.IsDraw,
                IsOver = notification.IsOver,
                Board = notification.Board
                    .Select(c => new GameHubCell(c.Row, c.Col, c.Mark))
                    .ToList()
            };

            OnGameStateInitialized?.Invoke(response);
        });
        
        return Task.CompletedTask;
    }
    
    protected override string HubUrl { get; }
    protected override string HubName { get; } = "game";
    
    public GameHubClient(IConfiguration configuration)
    {
        HubUrl = configuration.GetValue<string>("Services:gamenotificationservice:https:0")
                 ?? configuration.GetValue<string>("Services:gamenotificationservice:http:0")
                 ?? "https://localhost:7293";
        
    }
    
    public virtual async Task JoinGame(string gameId)
    {
        if (Connection is not null)
        {
            await Connection.InvokeAsync("JoinGame", gameId);
        }
    }

    public virtual async Task LeaveGame(string gameId)
    {
        if (Connection is not null)
        {
            await Connection.InvokeAsync("LeaveGame", gameId);
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
