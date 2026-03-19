using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using UserService.Features.Users.Entities;

namespace UserService.Services;

public sealed class UserCacheService(IDistributedCache cache) : IUserCacheService
{
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
    };

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public Task<UserEntity?> GetUserAsync(Guid id, CancellationToken ct = default)
        => GetRecordAsync<UserEntity>(GetUserKey(id), ct);

    public Task SetUserAsync(UserEntity user, CancellationToken ct = default)
        => SetRecordAsync(GetUserKey(user.Id), user, CacheOptions, ct);

    public Task<IReadOnlyList<UserEntity>?> GetUsersAsync(CancellationToken ct = default)
        => GetRecordAsync<IReadOnlyList<UserEntity>>(UsersKey, ct);

    public Task SetUsersAsync(IReadOnlyList<UserEntity> users, CancellationToken ct = default)
        => SetRecordAsync(UsersKey, users, CacheOptions, ct);

    private static string GetUserKey(Guid id) => $"users:{id:D}";

    private const string UsersKey = "users:list";

    private async Task<T?> GetRecordAsync<T>(string key, CancellationToken ct)
    {
        var payload = await cache.GetStringAsync(key, ct);
        return payload is null ? default : JsonSerializer.Deserialize<T>(payload, JsonOptions);
    }

    private Task SetRecordAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken ct)
    {
        var payload = JsonSerializer.Serialize(value, JsonOptions);
        return cache.SetStringAsync(key, payload, options, ct);
    }
}
