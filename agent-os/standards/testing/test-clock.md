# TestClock

Inject `IClock` into any service that uses the current time. In tests, use a concrete `TestClock` fake with `Advance()` for deterministic time control.

**Production registration:**
```csharp
services.AddSingleton<IClock, SystemClock>();
```

**Test fake:**
```csharp
private sealed class TestClock : IClock
{
    private DateTimeOffset _now = DateTimeOffset.UtcNow;
    public DateTimeOffset UtcNow => _now;
    public void Advance(TimeSpan by) => _now = _now.Add(by);
}
```

**Usage in tests:**
```csharp
var clock = new TestClock();
var service = new PersonStorageService(cache, clock);

var id = await service.CreateAsync(entity, ct);
clock.Advance(TimeSpan.FromSeconds(1));
await service.UpdateAsync(updated, ct);

var result = await service.GetAsync(id, ct);
result.UpdatedAt.Should().BeAfter(result.CreatedAt.Value);
```

- Never use `DateTime.UtcNow` or `DateTimeOffset.UtcNow` directly in services — always inject `IClock`
- `TestClock` is defined as a `private sealed class` inside the test class