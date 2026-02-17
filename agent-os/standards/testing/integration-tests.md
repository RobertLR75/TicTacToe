# Integration Tests

Integration tests share one `TestAppFactory` per class and reset state between each test.

```csharp
[assembly: CollectionBehavior(DisableTestParallelization = true)]

public class CreatePersonEndpointTests : IClassFixture<TestAppFactory>, IAsyncLifetime
{
    private readonly TestAppFactory _factory;

    public CreatePersonEndpointTests(TestAppFactory factory) => _factory = factory;

    public ValueTask InitializeAsync() { _factory.ResetState(); return ValueTask.CompletedTask; }
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task CreatePerson_Returns200()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = _factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });

        var response = await client.PostJsonAsync("/persons", new CreatePersonRequest { ... }, ct);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

- `DisableTestParallelization = true` is assembly-wide — shared MemoryCache state would corrupt parallel runs
- `ResetState()` clears shared state cheaply; spinning up a new `WebApplication` per test would be slow
- Each test method creates its own `HttpClient` with `using` (disposed after the test)
- `BaseAddress` is always `https://localhost`