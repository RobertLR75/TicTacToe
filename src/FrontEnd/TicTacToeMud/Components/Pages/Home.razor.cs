using Microsoft.AspNetCore.Components;
using TicTacToeMud.Models;
using TicTacToeMud.Services;
using TicTacToeMud.Session;

namespace TicTacToeMud.Components.Pages;

public partial class Home
{
    [Inject] private GameApiClient GameApi { get; set; } = default!;
    [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;

    [PersistentState] public string? PersistedPlayerId { get; set; }
    [PersistentState] public string? PersistedPlayerName { get; set; }

    private bool _isLoadingGames;
    private bool _isCreatingGame;
    private string? _playerId;
    private string? _playerName;
    private List<HomeGameListItemViewModel> _games = [];

    protected override async Task OnInitializedAsync()
    {
        ResolvePlayerIdentity();
        await LoadGamesAsync();
    }

    private void ResolvePlayerIdentity()
    {
        if (!string.IsNullOrWhiteSpace(PersistedPlayerId) && !string.IsNullOrWhiteSpace(PersistedPlayerName))
        {
            _playerId = PersistedPlayerId;
            _playerName = PersistedPlayerName;
            return;
        }

        var context = HttpContextAccessor.HttpContext;
        if (context is null)
        {
            return;
        }

        if (SessionUserStore.TryRead(context, out var user))
        {
            _playerId = user.UserId.ToString();
            _playerName = user.Name;
            PersistedPlayerId = _playerId;
            PersistedPlayerName = _playerName;
        }
    }

    async Task OnNewGame()
    {
        if (_isLoadingGames || _isCreatingGame)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_playerId) || string.IsNullOrWhiteSpace(_playerName))
        {
            ResolvePlayerIdentity();
        }

        if (string.IsNullOrWhiteSpace(_playerId) || string.IsNullOrWhiteSpace(_playerName))
        {
            NotificationService.ShowError("Missing player identity. Please sign in again.");
            return;
        }

        _isCreatingGame = true;

        try
        {
            var gameId = await GameApi.CreateGameAsync(_playerId, _playerName);
            NotificationService.ShowSuccess($"Game created: {gameId}");
            await LoadGamesAsync();
        }
        catch (Exception ex)
        {
            NotificationService.ShowError($"Failed to create game: {ex.Message}");
        }
        finally
        {
            _isCreatingGame = false;
        }
    }
    private async Task LoadGamesAsync()
    {
        _isLoadingGames = true;

        try
        {
            var games = await GameApi.ListGamesAsync();
            _games = games.Select(MapToViewModel).ToList();
        }
        catch (Exception ex)
        {
            NotificationService.ShowError($"Failed to load games: {ex.Message}");
        }
        finally
        {
            _isLoadingGames = false;
        }
    }

    private static HomeGameListItemViewModel MapToViewModel(GameListItem game)
    {
        return new HomeGameListItemViewModel
        {
            GameId = game.Id.ToString(),
            StatusLabel = GetStatusLabel(game.Status),
            PlayerName = game.Player1.Name,
            CreatedAtLabel = game.CreatedAt.LocalDateTime.ToString("g")
        };
    }

    private static string GetStatusLabel(int status)
    {
        return status switch
        {
            0 => "Created",
            1 => "InProgress",
            2 => "Completed",
            _ => $"Status {status}"
        };
    }

    internal void SetPlayerIdentityForTest(string playerId, string playerName)
    {
        PersistedPlayerId = playerId;
        PersistedPlayerName = playerName;
        _playerId = playerId;
        _playerName = playerName;
    }
}
