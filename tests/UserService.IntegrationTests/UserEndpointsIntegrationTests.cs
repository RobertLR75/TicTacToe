using System.Net;
using System.Net.Http.Json;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using Service.Contracts.Shared;
using UserService.IntegrationTests.Testing;
using Xunit;

namespace UserService.IntegrationTests;

[Collection(UserServiceCollection.Name)]
public sealed class UserEndpointsIntegrationTests(CosmosDbFixture cosmosFixture, RabbitMqFixture rabbitMqFixture)
{
    [Fact]
    public async Task Create_get_update_and_list_endpoints_work_end_to_end()
    {
        await using var factory = new UserServiceWebApplicationFactory(cosmosFixture.ConnectionString, rabbitMqFixture.ConnectionString);
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/users", new CreateUserRequest { Name = "Alice" });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<UserModel>();
        Assert.NotNull(created);
        Assert.Equal("Alice", created.Name);
        Assert.Equal(UserStatusEnum.Active, created.Status);

        var getResponse = await client.GetAsync($"/api/users/{created.UserId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await getResponse.Content.ReadFromJsonAsync<UserModel>();
        Assert.NotNull(fetched);
        Assert.Equal(created.UserId, fetched.UserId);

        var userId = Guid.Parse(created.UserId);

        var updateResponse = await client.PutAsJsonAsync($"/api/users/{created.UserId}", new UpdateUserRequest { Id = userId, Name = "Bob" });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await updateResponse.Content.ReadFromJsonAsync<UserModel>();
        Assert.NotNull(updated);
        Assert.Equal("Bob", updated.Name);
        Assert.Equal(UserStatusEnum.Active, updated.Status);

        var disableResponse = await client.PutAsJsonAsync($"/api/users/{created.UserId}/status", new UpdateUserStatusRequest
        {
            Id = userId,
            Status = UserStatusEnum.Disabled
        });
        Assert.Equal(HttpStatusCode.OK, disableResponse.StatusCode);

        var disabled = await disableResponse.Content.ReadFromJsonAsync<UpdateUserStatusResponse>();
        Assert.NotNull(disabled);
        Assert.Equal(UserStatusEnum.Disabled, disabled.Status);

        var listResponse = await client.GetAsync("/api/users");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var users = await listResponse.Content.ReadFromJsonAsync<ListUsersResponse>();
        Assert.NotNull(users);
        Assert.Contains(users!.Users, user => user.UserId == created.UserId && user.Name == "Bob" && user.Status == UserStatusEnum.Disabled);
    }

    [Fact]
    public async Task Create_and_update_validate_payloads()
    {
        await using var factory = new UserServiceWebApplicationFactory(cosmosFixture.ConnectionString, rabbitMqFixture.ConnectionString);
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/users", new CreateUserRequest { Name = string.Empty });
        Assert.Equal(HttpStatusCode.BadRequest, createResponse.StatusCode);

        var updateResponse = await client.PutAsJsonAsync($"/api/users/{Guid.NewGuid()}", new UpdateUserRequest { Id = Guid.Empty, Name = string.Empty });
        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);

        var disableResponse = await client.PutAsJsonAsync($"/api/users/{Guid.NewGuid()}/status", new UpdateUserStatusRequest { Id = Guid.Empty, Status = UserStatusEnum.Active });
        Assert.Equal(HttpStatusCode.BadRequest, disableResponse.StatusCode);
    }
}
