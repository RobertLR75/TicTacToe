using Bunit;
using Bunit.JSInterop;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using TicTacToeMud.Components.Pages;
using TicTacToeMud.Models;
using TicTacToeMud.Services;
using TicTacToeMud.Session;
using Xunit;

namespace TicTacToeMud.Tests;

public sealed class GamePageClientUsageTests : TestContext
{
    public GamePageClientUsageTests()
    {
        Services.AddMudServices();
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IHttpContextAccessor>(CreateHttpContextAccessor());
        Services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
    }

    [Fact]
    public async Task Game_page_uses_game_state_client_for_initial_state_load_after_create()
    {
        var gameApi = new StubGameApiClient();
        var gameStateApi = new StubGameStateServiceClient();

        Services.AddSingleton<GameApiClient>(gameApi);
        Services.AddSingleton<GameStateServiceClient>(gameStateApi);
        Services.AddSingleton<GameHubClient>(new StubGameHubClient());
        Services.AddSingleton<INotificationService>(new TestNotificationService());

        var cut = RenderComponent<Game>();

        await cut.Instance.TriggerNewGameForTestAsync("player-1", "Alice");

        Assert.Equal(1, gameApi.CreateGameCalls);
        Assert.Equal(1, gameApi.ListGamesCalls);
        Assert.Equal(1, gameStateApi.GetGameCalls);
        Assert.Equal("game-created-123", gameStateApi.LastGameId);
    }

    [Fact]
    public async Task Game_page_uses_game_state_client_for_move_submission()
    {
        var gameApi = new StubGameApiClient();
        var gameStateApi = new StubGameStateServiceClient();

        Services.AddSingleton<GameApiClient>(gameApi);
        Services.AddSingleton<GameStateServiceClient>(gameStateApi);
        Services.AddSingleton<GameHubClient>(new StubGameHubClient());
        Services.AddSingleton<INotificationService>(new TestNotificationService());

        var cut = RenderComponent<Game>();

        cut.Instance.SetGameStateForTest(
            "game-created-123",
            new GameResponse
            {
                GameId = "game-created-123",
                CurrentPlayer = 1,
                Winner = 0,
                IsDraw = false,
                IsOver = false,
                Board = []
            });

        await cut.Instance.TriggerCellClickForTestAsync(0, 1);

        Assert.Equal(0, gameApi.CreateGameCalls);
        Assert.Equal(1, gameApi.ListGamesCalls);
        Assert.Equal(1, gameStateApi.MakeMoveCalls);
        Assert.Equal("game-created-123", gameStateApi.LastMoveGameId);
        Assert.Equal(0, gameStateApi.LastMoveRow);
        Assert.Equal(1, gameStateApi.LastMoveCol);
    }

    [Fact]
    public void Game_page_loads_selected_game_from_route_without_creating_a_new_game()
    {
        var gameApi = new StubGameApiClient();
        var gameStateApi = new StubGameStateServiceClient();
        var gameHub = new StubGameHubClient();

        Services.AddSingleton<GameApiClient>(gameApi);
        Services.AddSingleton<GameStateServiceClient>(gameStateApi);
        Services.AddSingleton<GameHubClient>(gameHub);
        Services.AddSingleton<INotificationService>(new TestNotificationService());

        var cut = RenderComponent<Game>(parameters => parameters
            .Add(component => component.SelectedGameId, "existing-game-123"));

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(0, gameApi.CreateGameCalls);
            Assert.Equal(1, gameApi.ListGamesCalls);
            Assert.Equal(1, gameStateApi.GetGameCalls);
            Assert.Equal("existing-game-123", gameStateApi.LastGameId);
            Assert.Equal(1, gameHub.JoinGameCalls);
            Assert.Equal("existing-game-123", gameHub.LastJoinedGameId);
        });
    }

    private sealed class StubGameApiClient : GameApiClient
    {
        public StubGameApiClient() : base(new HttpClient { BaseAddress = new Uri("https://example.test") })
        {
        }

        public int CreateGameCalls { get; private set; }
        public int ListGamesCalls { get; private set; }

        public override Task<string> CreateGameAsync(string playerId, string playerName)
        {
            CreateGameCalls++;
            return Task.FromResult("game-created-123");
        }

        public override Task<IReadOnlyList<GameListItem>> ListGamesAsync()
        {
            ListGamesCalls++;
            return Task.FromResult<IReadOnlyList<GameListItem>>([]);
        }
    }

    private sealed class StubGameStateServiceClient : GameStateServiceClient
    {
        public StubGameStateServiceClient() : base(new HttpClient { BaseAddress = new Uri("https://example.test") })
        {
        }

        public int GetGameCalls { get; private set; }
        public string? LastGameId { get; private set; }
        public int MakeMoveCalls { get; private set; }
        public string? LastMoveGameId { get; private set; }
        public int LastMoveRow { get; private set; }
        public int LastMoveCol { get; private set; }

        public override Task<GameResponse> GetGameAsync(string gameId)
        {
            GetGameCalls++;
            LastGameId = gameId;

            return Task.FromResult(new GameResponse
            {
                GameId = gameId,
                CurrentPlayer = 1,
                Winner = 0,
                IsDraw = false,
                IsOver = false,
                Board = []
            });
        }

        public override Task MakeMoveAsync(string gameId, int row, int col)
        {
            MakeMoveCalls++;
            LastMoveGameId = gameId;
            LastMoveRow = row;
            LastMoveCol = col;
            return Task.CompletedTask;
        }
    }

    private sealed class StubGameHubClient : GameHubClient
    {
        public StubGameHubClient() : base(new ConfigurationBuilder().Build())
        {
        }

        public int JoinGameCalls { get; private set; }
        public string? LastJoinedGameId { get; private set; }

        public override Task StartAsync(string? hubBaseUrl = null) => Task.CompletedTask;

        public override Task JoinGame(string gameId)
        {
            JoinGameCalls++;
            LastJoinedGameId = gameId;
            return Task.CompletedTask;
        }

        public override Task LeaveGame(string gameId) => Task.CompletedTask;
    }

    private sealed class TestNotificationService : INotificationService
    {
        public void ShowSuccess(string message)
        {
        }

        public void ShowError(string message)
        {
        }

        public void ShowWarning(string message)
        {
        }
    }

    private static IHttpContextAccessor CreateHttpContextAccessor()
    {
        var context = new DefaultHttpContext();
        context.Features.Set<ISessionFeature>(new SessionFeature { Session = new TestSession() });
        SessionUserStore.Write(context.Session, new SessionUser(Guid.Parse("6f8ba576-2c55-4ea2-88ad-c2f783d0beec"), "Alice"));
        return new HttpContextAccessor { HttpContext = context };
    }

    private sealed class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = [];

        public IEnumerable<string> Keys => _store.Keys;
        public string Id => "test-session";
        public bool IsAvailable => true;

        public void Clear() => _store.Clear();

        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Remove(string key) => _store.Remove(key);

        public void Set(string key, byte[] value) => _store[key] = value;

        public bool TryGetValue(string key, out byte[]? value)
        {
            var found = _store.TryGetValue(key, out var stored);
            value = stored;
            return found;
        }
    }

    private sealed class SessionFeature : ISessionFeature
    {
        public ISession Session { get; set; } = default!;
    }
}
