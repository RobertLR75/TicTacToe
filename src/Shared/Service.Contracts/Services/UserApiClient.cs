using System.Net;
using System.Net.Http.Json;
using Service.Contracts.Requests;
using Service.Contracts.Responses;

namespace Service.Contracts.Services;

public sealed class UserApiClient(IHttpClientFactory httpClientFactory)
{
    private const string ClientName = "users";

    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<UserModel> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        using var response = await client.PostAsJsonAsync("/api/users", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<UserModel>(cancellationToken: cancellationToken))!;
    }

    public async Task<UserModel?> GetUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        using var response = await client.GetAsync($"/api/users/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserModel>(cancellationToken: cancellationToken);
    }

    public async Task<List<UserModel>> ListUsersAsync(CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        using var response = await client.GetAsync("/api/users", cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<ListUsersResponse>(cancellationToken: cancellationToken);
        return payload?.Users ?? [];
    }

    public async Task<UserModel> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        using var response = await client.PutAsJsonAsync($"/api/users/{id}", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<UserModel>(cancellationToken: cancellationToken))!;
    }

    public Task<UpdateUserStatusResponse> UpdateUserStatusAsync(Guid id, UpdateUserStatusRequest request, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        return UpdateUserStatusInternalAsync(client, id, request, cancellationToken);
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient(ClientName);

    private static async Task<UpdateUserStatusResponse> UpdateUserStatusInternalAsync(HttpClient client, Guid id, UpdateUserStatusRequest request, CancellationToken cancellationToken)
    {
        var response = await client.PutAsJsonAsync($"/api/users/{id}/status", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<UpdateUserStatusResponse>(cancellationToken: cancellationToken))!;
    }
}
