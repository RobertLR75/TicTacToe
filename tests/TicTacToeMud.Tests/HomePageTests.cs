using Bunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using TicTacToeMud.Components.Pages;
using TicTacToeMud.Models;
using TicTacToeMud.Services;
using TicTacToeMud.Session;
using Xunit;

namespace TicTacToeMud.Tests;

public sealed class HomePageTests : TestContext
{
    public HomePageTests()
    {
        Services.AddMudServices();
    }

    [Fact]
    public void Home_loads_games_on_first_render_and_displays_list_items()
    {
        var api = new StubGameApiClient
        {
            ListedGames =
            [
                new GameListItem
                {
                    Id = Guid.Parse("2a08ef26-61f1-4304-8db3-9b43db8ad547"),
                    Status = 0,
                    CreatedAt = new DateTimeOffset(2026, 1, 1, 10, 0, 0, TimeSpan.Zero),
                    UpdatedAt = null,
                    Player1 = new PlayerListItem { Id = "player-1", Name = "Alice" },
                    Player2 = null
                }
            ]
        };

        var notificationService = new TestNotificationService();
        Services.AddSingleton<GameApiClient>(api);
        Services.AddSingleton<INotificationService>(notificationService);
        Services.AddSingleton<IHttpContextAccessor>(CreateHttpContextAccessor("Alice"));

        var cut = RenderComponent<Home>();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(1, api.ListGamesCalls);
            var markup = cut.Markup;
            Assert.Contains("Created Games", markup);
            Assert.Contains("Alice", markup);
        });
    }

    [Fact]
    public void Home_shows_empty_state_when_list_games_returns_no_items()
    {
        var api = new StubGameApiClient { ListedGames = [] };

        Services.AddSingleton<GameApiClient>(api);
        Services.AddSingleton<INotificationService>(new TestNotificationService());
        Services.AddSingleton<IHttpContextAccessor>(CreateHttpContextAccessor("Alice"));

        var cut = RenderComponent<Home>();

        cut.WaitForAssertion(() => Assert.Contains("No games yet. Create one to get started.", cut.Markup));
    }

    [Fact]
    public void Home_new_game_creates_game_and_refreshes_list()
    {
        var api = new StubGameApiClient
        {
            ListedGames =
            [
                new GameListItem
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Status = 0,
                    CreatedAt = new DateTimeOffset(2026, 1, 1, 10, 0, 0, TimeSpan.Zero),
                    UpdatedAt = null,
                    Player1 = new PlayerListItem { Id = "player-1", Name = "Alice" },
                    Player2 = null
                }
            ]
        };

        var notificationService = new TestNotificationService();
        Services.AddSingleton<GameApiClient>(api);
        Services.AddSingleton<INotificationService>(notificationService);
        Services.AddSingleton<IHttpContextAccessor>(CreateHttpContextAccessor("Alice", Guid.Parse("7d9173f3-564f-4fdc-abfe-e98936e089f6")));

        var cut = RenderComponent<Home>();
        cut.WaitForAssertion(() => Assert.Equal(1, api.ListGamesCalls));

        cut.Find("[data-testid='home-new-game-button']").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(1, api.CreateGameCalls);
            Assert.Equal(2, api.ListGamesCalls);
            Assert.Equal("7d9173f3-564f-4fdc-abfe-e98936e089f6", api.LastCreatePlayerId);
            Assert.Equal("Alice", api.LastCreatePlayerName);
            Assert.Contains(notificationService.SuccessMessages, m => m.Contains("Game created:"));
        });
    }

    [Fact]
    public void Home_shows_error_notification_when_list_games_fails()
    {
        var api = new StubGameApiClient { ListGamesException = new InvalidOperationException("list-failed") };
        var notificationService = new TestNotificationService();

        Services.AddSingleton<GameApiClient>(api);
        Services.AddSingleton<INotificationService>(notificationService);
        Services.AddSingleton<IHttpContextAccessor>(CreateHttpContextAccessor("Alice"));

        _ = RenderComponent<Home>();

        Assert.Contains(notificationService.ErrorMessages, m => m.Contains("Failed to load games: list-failed"));
    }

    [Fact]
    public void Home_shows_error_notification_when_create_fails_and_list_is_unchanged()
    {
        var api = new StubGameApiClient
        {
            ListedGames =
            [
                new GameListItem
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    Status = 0,
                    CreatedAt = new DateTimeOffset(2026, 1, 1, 10, 0, 0, TimeSpan.Zero),
                    UpdatedAt = null,
                    Player1 = new PlayerListItem { Id = "player-1", Name = "Alice" },
                    Player2 = null
                }
            ],
            CreateGameException = new InvalidOperationException("create-failed")
        };

        var notificationService = new TestNotificationService();
        Services.AddSingleton<GameApiClient>(api);
        Services.AddSingleton<INotificationService>(notificationService);
        Services.AddSingleton<IHttpContextAccessor>(CreateHttpContextAccessor("Alice"));

        var cut = RenderComponent<Home>();
        cut.WaitForAssertion(() => Assert.Equal(1, api.ListGamesCalls));

        cut.Find("[data-testid='home-new-game-button']").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(1, api.CreateGameCalls);
            Assert.Equal(1, api.ListGamesCalls);
            Assert.Contains(notificationService.ErrorMessages, m => m.Contains("Failed to create game: create-failed"));
            Assert.Contains("Alice", cut.Markup);
        });
    }

    private static IHttpContextAccessor CreateHttpContextAccessor(string playerName, Guid? playerId = null)
    {
        var context = new DefaultHttpContext();
        context.Features.Set<ISessionFeature>(new SessionFeature { Session = new TestSession() });

        var user = new SessionUser(playerId ?? Guid.Parse("6f8ba576-2c55-4ea2-88ad-c2f783d0beec"), playerName);
        SessionUserStore.Write(context.Session, user);

        return new HttpContextAccessor { HttpContext = context };
    }

    private sealed class StubGameApiClient : GameApiClient
    {
        public StubGameApiClient() : base(new HttpClient { BaseAddress = new Uri("https://example.test") })
        {
        }

        public int ListGamesCalls { get; private set; }
        public int CreateGameCalls { get; private set; }
        public string? LastCreatePlayerId { get; private set; }
        public string? LastCreatePlayerName { get; private set; }
        public IReadOnlyList<GameListItem> ListedGames { get; set; } = [];
        public Exception? ListGamesException { get; set; }
        public Exception? CreateGameException { get; set; }

        public override Task<IReadOnlyList<GameListItem>> ListGamesAsync()
        {
            ListGamesCalls++;

            if (ListGamesException is not null)
            {
                throw ListGamesException;
            }

            return Task.FromResult(ListedGames);
        }

        public override Task<string> CreateGameAsync(string playerId, string playerName)
        {
            CreateGameCalls++;
            LastCreatePlayerId = playerId;
            LastCreatePlayerName = playerName;

            if (CreateGameException is not null)
            {
                throw CreateGameException;
            }

            return Task.FromResult("game-created-123");
        }
    }

    private sealed class TestNotificationService : INotificationService
    {
        public List<string> SuccessMessages { get; } = [];
        public List<string> ErrorMessages { get; } = [];
        public List<string> WarningMessages { get; } = [];

        public void ShowSuccess(string message)
        {
            SuccessMessages.Add(message);
        }

        public void ShowError(string message)
        {
            ErrorMessages.Add(message);
        }

        public void ShowWarning(string message)
        {
            WarningMessages.Add(message);
        }
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
