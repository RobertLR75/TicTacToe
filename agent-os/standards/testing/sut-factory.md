# SUT Factory Methods

Create the System Under Test via a private static factory method. This makes test setup explicit and reusable within the class.

```csharp
// Single-value SUT
private static UpdatePersonMapper Mapper() => new();

// Multi-dependency SUT — return a tuple
private static (IMemoryCache Cache, TestClock Clock, IPersonStorageService Service) Sut()
{
    var cache = new MemoryCache(new MemoryCacheOptions());
    var clock = new TestClock();
    return (cache, clock, new MemoryCachePersonStorageService(cache, clock));
}

// Usage in test
[Fact]
public async Task CreateAsync_AssignsId()
{
    var (_, clock, service) = Sut();
    var ct = TestContext.Current.CancellationToken;
    // ...
}
```

- Factory methods are `private static` — pure utility, no instance state
- Return tuples when the test needs access to dependencies (e.g. `TestClock` to advance time)
- No shared state in test class constructors — each test builds its own SUT