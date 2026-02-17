# HTTP Client Helpers

All integration test HTTP calls use `HttpClientJsonExtensions` to ensure consistent JSON serialization.

```csharp
internal static class HttpClientJsonExtensions
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public static Task<HttpResponseMessage> PostJsonAsync<T>(
        this HttpClient client, string url, T body, CancellationToken ct)
        => client.PostAsJsonAsync(url, body, JsonOptions, ct);

    public static Task<T?> ReadJsonAsync<T>(
        this HttpContent content, CancellationToken ct)
        => content.ReadFromJsonAsync<T>(JsonOptions, ct);
}
```

**Usage:**
```csharp
var response = await client.PostJsonAsync("/persons", request, ct);
var result = await response.Content.ReadJsonAsync<CreatePersonResponse>(ct);
```

- Never call `PostAsJsonAsync`/`ReadFromJsonAsync` directly in tests — use the extensions
- JSON options use `JsonSerializerDefaults.Web` (camelCase, case-insensitive)
