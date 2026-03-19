using System.Net;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Service.Contracts.Models;
using Service.Contracts.Services;
using TicTacToeMud.Models;
using TicTacToeMud.Services;

namespace TicTacToeMud.Components.Pages;

public partial class Game : IAsyncDisposable
{
    [Inject] private GameApiClient GameApi { get; set; } = default!;
    [Inject] private GameStateServiceClient GameStateApi { get; set; } = default!;
    [Inject] private GameHubClient GameHub { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;

    private string?[,] _board = new string?[3, 3];
    private string? _gameId;
    // private string? _loadedSelectedGameId;
    private bool _isOver;
    private bool _isDraw;
    private string? _winner;
    private string? _currentPlayer;
    private bool _isLoading;
    private bool _showOutcomeBanner;
    private bool _outcomeFading;
    private CancellationTokenSource? _outcomeFadeCts;
    private const int OutcomeVisibleMs = 2000;
    private const int OutcomeFadeMs = 350;
    private const string WinCelebrationImageUrl = "https://cdn.jsdelivr.net/gh/twitter/twemoji@14.0.2/assets/svg/1f389.svg";
    private const string DrawCelebrationImageUrl = "https://cdn.jsdelivr.net/gh/twitter/twemoji@14.0.2/assets/svg/1f91d.svg";

    [Parameter]
    public string? SelectedGameId { get; set; }

    private string StatusMessage
    {
        get
        {
            if (_gameId is null)
            {
                return $"Select a created game from Home to start playing.";
            }

            if (_isLoading)
            {
                return "Loading...";
            }

            if (_isDraw)
            {
                return "It's a draw!";
            }

            return _winner is not null ? $"Player {_winner} wins!" : $"Player {_currentPlayer}'s turn";
        }
    }

    private string OutcomeContainerClass
        => _isDraw ? "game-outcome draw" : _winner == "O" ? "game-outcome win-o" : "game-outcome win-x";

    private string OutcomeOverlayClass => _outcomeFading ? "outcome-overlay is-fading" : "outcome-overlay";

    private string OutcomeAnimationClass => _outcomeFading ? $"{OutcomeContainerClass} is-fading" : OutcomeContainerClass;

    private string OutcomeTitle => _isDraw ? "Stalemate!" : $"Player {_winner} wins!";

    private string OutcomeSubtitle
        => _isDraw
            ? "Both players held strong. Nice defensive game."
            : _winner == "X"
                ? "X storms the board with a clutch finish!"
                : "O takes the round with smooth strategy!";

    private string OutcomeIcon => _isDraw ? Icons.Material.Filled.Balance : Icons.Material.Filled.EmojiEvents;

    private string OutcomeImageUrl => _isDraw ? DrawCelebrationImageUrl : WinCelebrationImageUrl;

    private string OutcomeImageAlt => _isDraw ? "Draw celebration" : "Win celebration";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        await ConnectToHub();
    }

    protected override async Task OnParametersSetAsync()
    {
        await LoadSelectedGameAsync();
    }

    private async Task ConnectToHub()
    {
        try
        {
            GameHub.OnGameStateUpdated += HandleGameStateUpdated;
            GameHub.OnGameStateInitialized += HandleGameStateInitialized;
            await GameHub.StartAsync();

            if (_gameId is not null)
            {
                await GameHub.Join(_gameId);
            }

            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            NotificationService.ShowWarning($"Failed to connect to game hub: {ex.Message}");
        }
    }

    private void HandleGameStateUpdated(GameHubModel response)
    {
        if (response.GameId != _gameId)
        {
            return;
        }

        _ = InvokeAsync(() =>
        {
            UpdateState(response.ToGameResponse());
            _isLoading = false;
            StateHasChanged();
        });
    }

    private void HandleGameStateInitialized(GameHubModel response)
    {
        if (response.GameId != _gameId)
        {
            return;
        }

        _ = InvokeAsync(() =>
        {
            UpdateState(response.ToGameResponse());
            _isLoading = false;
            StateHasChanged();
        });
    }

    private async Task LoadSelectedGameAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedGameId) || SelectedGameId == _gameId)
        {
            return;
        }

        if (!Guid.TryParse(SelectedGameId, out _))
        {
            ResetState();
            NavigationManager.NavigateTo("/not-found");
            return;
        }

        _isLoading = true;

        try
        {
            var previousGameId = _gameId;

            if (previousGameId is not null && previousGameId != SelectedGameId)
            {
                await GameHub.Leave(previousGameId);
            }

            var state = await GameStateApi.GetGameAsync(SelectedGameId);
            _gameId = SelectedGameId;

            await GameHub.Join(SelectedGameId);

            UpdateState(state);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            ResetState();
            NavigationManager.NavigateTo("/not-found");
        }
        catch (Exception ex)
        {
            NotificationService.ShowError($"Failed to load game: {ex.Message}");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task OnCellClick(int row, int col)
    {
        if (_gameId is null || _isOver || _board[row, col] is not null)
        {
            return;
        }

        _isLoading = true;

        try
        {
            await GameStateApi.MakeMoveAsync(_gameId, row, col);
        }
        catch (Exception ex)
        {
            _isLoading = false;
            NotificationService.ShowError($"Failed to make move: {ex.Message}");
        }
    }

    internal Task TriggerCellClickForTestAsync(int row, int col)
    {
        return OnCellClick(row, col);
    }

    internal void SetGameStateForTest(string gameId, GameResponse response)
    {
        _gameId = gameId;
        UpdateState(response);
    }

    internal void SetPlayerIdentityForTest(string playerId, string playerName)
    {
        ArgumentNullException.ThrowIfNull(playerId);
        ArgumentNullException.ThrowIfNull(playerName);
    }

    private void UpdateState(GameResponse response)
    {
        var wasOver = _isOver;

        _gameId = response.GameId;
        _isOver = response.IsOver;
        _isDraw = response.IsDraw;
        _winner = MarkToString(response.Winner);
        _currentPlayer = MarkToString(response.CurrentPlayer);

        _board = new string?[3, 3];
        foreach (var cell in response.Board)
        {
            _board[cell.Row, cell.Col] = MarkToString(cell.Mark);
        }

        if (_isOver && !wasOver)
        {
            ShowOutcomeBannerWithFade();
        }

        if (!_isOver)
        {
            HideOutcomeBanner();
        }
    }

    private void ResetState()
    {
        _gameId = null;
        _isOver = false;
        _isDraw = false;
        _winner = null;
        _currentPlayer = null;
        _board = new string?[3, 3];

        HideOutcomeBanner();
    }

    private void ShowOutcomeBannerWithFade()
    {
        _outcomeFadeCts?.Cancel();
        _outcomeFadeCts?.Dispose();
        _outcomeFadeCts = new CancellationTokenSource();

        _showOutcomeBanner = true;
        _outcomeFading = false;

        _ = FadeOutcomeBannerAsync(_outcomeFadeCts.Token);
    }

    private async Task FadeOutcomeBannerAsync(CancellationToken ct)
    {
        try
        {
            await Task.Delay(OutcomeVisibleMs, ct);

            await InvokeAsync(() =>
            {
                _outcomeFading = true;
                StateHasChanged();
            });

            await Task.Delay(OutcomeFadeMs, ct);

            await InvokeAsync(() =>
            {
                _showOutcomeBanner = false;
                _outcomeFading = false;
                StateHasChanged();
            });
        }
        catch (TaskCanceledException)
        {
            // ignored
        }
    }

    private void HideOutcomeBanner()
    {
        _outcomeFadeCts?.Cancel();
        _outcomeFadeCts?.Dispose();
        _outcomeFadeCts = null;
        _showOutcomeBanner = false;
        _outcomeFading = false;
    }

    private static string? MarkToString(int mark) => mark switch
    {
        1 => "X",
        2 => "O",
        _ => null
    };

    private string GetCellDisplay(int row, int col)
    {
        return _board[row, col] ?? "\u00A0";
    }

    private bool IsCellDisabled(int row, int col)
    {
        return _isOver || _board[row, col] is not null || _gameId is null || _isLoading;
    }

    public async ValueTask DisposeAsync()
    {
        HideOutcomeBanner();

        GameHub.OnGameStateUpdated -= HandleGameStateUpdated;
        GameHub.OnGameStateInitialized -= HandleGameStateInitialized;

        if (_gameId is not null)
        {
            await GameHub.Leave(_gameId);
        }
    }
}
