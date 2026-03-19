using Bunit;
using Bunit.JSInterop;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Service.Contracts.Responses;
using Service.Contracts.Services;
using TicTacToeMud.Components.Pages;
using TicTacToeMud.Models;
using TicTacToeMud.Services;
using TicTacToeMud.Session;
using Xunit;

namespace TicTacToeMud.Tests;

public sealed class GamePageClientUsageTests : BunitContext
{
    public GamePageClientUsageTests()
    {
        Services.AddMudServices();
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IHttpContextAccessor>(CreateHttpContextAccessor());
        Services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
    }

    [Fact]
    public void Game_page_disables_board_when_no_game_is_selected()
    {
        var gameApi = new StubGameApiClient();
        var gameStateApi = new StubGameStateServiceClient();

        Services.AddSingleton<GameApiClient>(gameApi);
        Services.AddSingleton<GameStateServiceClient>(gameStateApi);
        Services.AddSingleton<GameHubClient>(new StubGameHubClient());
        Services.AddSingleton<INotificationService>(new TestNotificationService());

        var navigationManager = Services.GetRequiredService<NavigationManager>();

        var cut = Render<Game>();

        var backButton = cut.Find("[data-testid='game-back-home-button']");

        Assert.Contains("No game selected. Create a game from Home, then open it to play.", cut.Markup);
        Assert.Contains("Select a created game from Home to start playing.", cut.Markup);
        Assert.Equal("/", backButton.GetAttribute("href"));
        Assert.DoesNotContain("Created Games", cut.Markup);
        Assert.DoesNotContain("New Game", cut.Markup);

        var cellButtons = cut.FindAll("button.mud-button-root");
        Assert.Equal(9, cellButtons.Count);
        Assert.All(cellButtons, button => Assert.True(button.HasAttribute("disabled")));

        Assert.Equal(0, gameApi.CreateGameCalls);
        Assert.Equal(0, gameApi.ListGamesCalls);
        Assert.Equal(0, gameApi.GetGameCalls);
        Assert.Equal(0, gameStateApi.GetGameCalls);
        Assert.Equal("http://localhost/", navigationManager.Uri);
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

        var cut = Render<Game>();

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
        Assert.Equal(0, gameApi.ListGamesCalls);
        Assert.Equal(0, gameApi.GetGameCalls);
        Assert.Equal(1, gameStateApi.MakeMoveCalls);
        Assert.Equal("game-created-123", gameStateApi.LastMoveGameId);
        Assert.Equal(0, gameStateApi.LastMoveRow);
        Assert.Equal(1, gameStateApi.LastMoveCol);
    }

    [Fact]
    public void Game_page_loads_selected_game_from_route_without_creating_a_new_game()
    {
        const string gameId = "2a08ef26-61f1-4304-8db3-9b43db8ad547";

        var gameApi = new StubGameApiClient();
        var gameStateApi = new StubGameStateServiceClient();
        var gameHub = new StubGameHubClient();

        Services.AddSingleton<GameApiClient>(gameApi);
        Services.AddSingleton<GameStateServiceClient>(gameStateApi);
        Services.AddSingleton<GameHubClient>(gameHub);
        Services.AddSingleton<INotificationService>(new TestNotificationService());

        var cut = Render<Game>(parameters => parameters
            .Add(component => component.SelectedGameId, gameId));

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(0, gameApi.CreateGameCalls);
            Assert.Equal(0, gameApi.ListGamesCalls);
            Assert.Equal(0, gameApi.GetGameCalls);
            Assert.Equal(1, gameStateApi.GetGameCalls);
            Assert.Equal(gameId, gameStateApi.LastGameId);
            Assert.Equal(1, gameHub.JoinGameCalls);
            Assert.Equal(gameId, gameHub.LastJoinedGameId);
            Assert.DoesNotContain("No game selected. Create a game from Home, then open it to play.", cut.Markup);
            Assert.Contains("Player X's turn", cut.Markup);
        });

        Assert.Contains(cut.FindAll("button.mud-button-root"), button => !button.HasAttribute("disabled"));
    }

    [Fact]
    public void Game_page_navigates_to_not_found_when_game_id_is_not_a_guid()
    {
        var gameApi = new StubGameApiClient();
        var gameStateApi = new StubGameStateServiceClient();
        var gameHub = new StubGameHubClient();

        Services.AddSingleton<GameApiClient>(gameApi);
        Services.AddSingleton<GameStateServiceClient>(gameStateApi);
        Services.AddSingleton<GameHubClient>(gameHub);
        Services.AddSingleton<INotificationService>(new TestNotificationService());

        var navigationManager = Services.GetRequiredService<NavigationManager>();

        _ = Render<Game>(parameters => parameters
            .Add(component => component.SelectedGameId, "not-a-guid"));

        Assert.Equal("http://localhost/not-found", navigationManager.Uri);
        Assert.Equal(0, gameApi.GetGameCalls);
        Assert.Equal(0, gameStateApi.GetGameCalls);
        Assert.Equal(0, gameHub.JoinGameCalls);
    }

    [Fact]
    public void Game_page_navigates_to_not_found_when_game_api_returns_missing_game()
    {
        var gameApi = new StubGameApiClient();
        var gameStateApi = new StubGameStateServiceClient
        {
            GetGameException = new HttpRequestException("missing", null, System.Net.HttpStatusCode.NotFound)
        };
        var gameHub = new StubGameHubClient();
        var notificationService = new TestNotificationService();

        Services.AddSingleton<GameApiClient>(gameApi);
        Services.AddSingleton<GameStateServiceClient>(gameStateApi);
        Services.AddSingleton<GameHubClient>(gameHub);
        Services.AddSingleton<INotificationService>(notificationService);

        var navigationManager = Services.GetRequiredService<NavigationManager>();

        _ = Render<Game>(parameters => parameters
            .Add(component => component.SelectedGameId, "2a08ef26-61f1-4304-8db3-9b43db8ad547"));

        Assert.Equal("http://localhost/not-found", navigationManager.Uri);
        Assert.Equal(0, gameApi.GetGameCalls);
        Assert.Equal(1, gameStateApi.GetGameCalls);
        Assert.Equal(0, gameHub.JoinGameCalls);
        Assert.Empty(notificationService.ErrorMessages);
    }

    private sealed class StubGameApiClient : GameApiClient
    {
        public StubGameApiClient() : base(new FixedHttpClientFactory(new HttpClient { BaseAddress = new Uri("https://example.test") }))
        {
        }

        public int CreateGameCalls { get; private set; }
        public int ListGamesCalls { get; private set; }
        public int GetGameCalls { get; private set; }
        public string? LastGameId { get; private set; }
        public Exception? GetGameException { get; init; }

        public override Task<string> CreateGameAsync(string playerId, string playerName, CancellationToken cancellationToken = default)
        {
            CreateGameCalls++;
            return Task.FromResult("game-created-123");
        }

        public override Task<IReadOnlyList<GameModel>> ListGamesAsync(CancellationToken cancellationToken = default)
        {
            ListGamesCalls++;
            return Task.FromResult<IReadOnlyList<GameModel>>([]);
        }

        public override Task<GetGameResponse> GetGameAsync(string gameId, CancellationToken cancellationToken = default)
        {
            GetGameCalls++;
            LastGameId = gameId;

            if (GetGameException is not null)
            {
                throw GetGameException;
            }

            return Task.FromResult(new GetGameResponse
            {
                GameId = gameId,
                Status = Service.Contracts.Shared.GameStatusEnum.Created,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = null,
                Player1 = new Service.Contracts.Shared.PlayerModel
                {
                    PlayerId = "11111111-1111-1111-1111-111111111111",
                    Name = "Alice"
                },
                Player2 = null
            });
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
        public Exception? GetGameException { get; init; }

        public override Task<GameResponse> GetGameAsync(string gameId)
        {
            GetGameCalls++;
            LastGameId = gameId;

            if (GetGameException is not null)
            {
                throw GetGameException;
            }

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

    private sealed class StubGameHubClient(IConfiguration configuration) : GameHubClient(configuration)
    {
        public StubGameHubClient() : this(new ConfigurationBuilder().Build())
        {
        }

        public int JoinGameCalls { get; private set; }
        public string? LastJoinedGameId { get; private set; }

        public Task StartAsync(string? hubBaseUrl = null) => Task.CompletedTask;

        public override Task Join(string gameId)
        {
            JoinGameCalls++;
            LastJoinedGameId = gameId;
            return Task.CompletedTask;
        }

        public override Task Leave(string gameId) => Task.CompletedTask;
    }

    private sealed class TestNotificationService : INotificationService
    {
        public List<string> ErrorMessages { get; } = [];

        public void ShowSuccess(string message)
        {
        }

        public void ShowError(string message)
        {
            ErrorMessages.Add(message);
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

    private sealed class FixedHttpClientFactory(HttpClient client) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => client;
    }
}
