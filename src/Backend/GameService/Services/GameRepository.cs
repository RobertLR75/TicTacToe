using System.Text.Json;
using GameService.Models;
using StackExchange.Redis;

namespace GameService.Services;

public class GameRepository
{
    private readonly IConnectionMultiplexer _redis;
    private const string GameKeyPrefix = "game:";
    private const string GameIndexKey = "game:index";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public GameRepository(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    private IDatabase Db => _redis.GetDatabase();

    public async Task<GameModel> CreateGameAsync(GameModel game, CancellationToken ct = default)
    {
        var key = GameKeyPrefix + game.Id;
        var json = JsonSerializer.Serialize(game, JsonOptions);

        var db = Db;
        await db.StringSetAsync(key, json);
        await db.SetAddAsync(GameIndexKey, game.Id);

        return game;
    }

    public async Task<GameModel?> GetGameAsync(string id, CancellationToken ct = default)
    {
        var key = GameKeyPrefix + id;
        var json = await Db.StringGetAsync(key);

        if (json.IsNullOrEmpty)
            return null;

        return JsonSerializer.Deserialize<GameModel>(json.ToString(), JsonOptions);
    }

    public async Task UpdateGameAsync(GameModel game, CancellationToken ct = default)
    {
        var key = GameKeyPrefix + game.Id;
        var json = JsonSerializer.Serialize(game, JsonOptions);
        await Db.StringSetAsync(key, json);
    }

    public async Task<List<GameModel>> GetGamesByStatusAsync(GameStatus status, CancellationToken ct = default)
    {
        var db = Db;
        var gameIds = await db.SetMembersAsync(GameIndexKey);
        var games = new List<GameModel>();

        foreach (var id in gameIds)
        {
            if (id.IsNullOrEmpty) continue;

            var game = await GetGameAsync(id!, ct);
            if (game is not null && game.Status == status)
            {
                games.Add(game);
            }
        }

        return games;
    }
}
