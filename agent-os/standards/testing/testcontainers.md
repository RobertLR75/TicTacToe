# Testcontainers

Always use Testcontainers for external infrastructure dependencies (Redis, MongoDB, Postgres). Never mock storage backends.

```csharp
public class BaseRedisCacheStorageServiceTests : IAsyncLifetime
{
    private IContainer _redisContainer = null!;
    private IPersonStorageService _service = null!;

    public async ValueTask InitializeAsync()
    {
        _redisContainer = new RedisBuilder("redis:7-alpine").Build();
        await _redisContainer.StartAsync();

        var mux = await ConnectionMultiplexer.ConnectAsync(_redisContainer.GetConnectionString());
        _service = new RedisCachePersonStorageService(mux, new TestClock());
    }

    public async ValueTask DisposeAsync()
    {
        await _redisContainer.StopAsync();
        await _redisContainer.DisposeAsync();
    }
}
```

- Container lifecycle managed via `IAsyncLifetime` (`InitializeAsync`/`DisposeAsync`)
- Pin the image tag (e.g. `redis:7-alpine`) for reproducibility
- Unit tests mock the `IStorageService` interface; only storage implementation tests use containers
