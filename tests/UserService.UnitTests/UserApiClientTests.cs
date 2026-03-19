using System.Net;
using System.Text;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using Service.Contracts.Shared;
using Service.Contracts.Services;
using Xunit;

namespace UserService.UnitTests;

public sealed class UserApiClientTests
{
    [Fact]
    public async Task CreateUserAsync_posts_to_expected_route()
    {
        HttpRequestMessage? capturedRequest = null;
        using var httpClient = CreateHttpClient(request =>
        {
            capturedRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"userId\":\"11111111-1111-1111-1111-111111111111\",\"name\":\"Alice\",\"status\":0}", Encoding.UTF8, "application/json")
            });
        });

        var sut = new UserApiClient(new FixedHttpClientFactory(httpClient));

        var response = await sut.CreateUserAsync(new CreateUserRequest { Name = "Alice" });

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest!.Method);
        Assert.Equal("https://example.test/api/users", capturedRequest.RequestUri!.ToString());
        Assert.Equal("Alice", response.Name);
        Assert.Equal(UserStatusEnum.Active, response.Status);
    }

    [Fact]
    public async Task ListUsersAsync_gets_expected_route()
    {
        HttpRequestMessage? capturedRequest = null;
        using var httpClient = CreateHttpClient(request =>
        {
            capturedRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"users\":[{\"userId\":\"11111111-1111-1111-1111-111111111111\",\"name\":\"Alice\",\"status\":0}]}", Encoding.UTF8, "application/json")
            });
        });

        var sut = new UserApiClient(new FixedHttpClientFactory(httpClient));

        var response = await sut.ListUsersAsync();

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Get, capturedRequest!.Method);
        Assert.Equal("https://example.test/api/users", capturedRequest.RequestUri!.ToString());
        Assert.NotNull(response);
        Assert.Single(response);
    }

    [Fact]
    public async Task UpdateUserStatusAsync_puts_to_status_route()
    {
        HttpRequestMessage? capturedRequest = null;
        using var httpClient = CreateHttpClient(request =>
        {
            capturedRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\":\"11111111-1111-1111-1111-111111111111\",\"status\":1}", Encoding.UTF8, "application/json")
            });
        });

        var sut = new UserApiClient(new FixedHttpClientFactory(httpClient));

        var response = await sut.UpdateUserStatusAsync(Guid.Parse("11111111-1111-1111-1111-111111111111"), new UpdateUserStatusRequest
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Status = UserStatusEnum.Disabled
        });

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Put, capturedRequest!.Method);
        Assert.Equal("https://example.test/api/users/11111111-1111-1111-1111-111111111111/status", capturedRequest.RequestUri!.ToString());
        Assert.Equal(UserStatusEnum.Disabled, response.Status);
    }

    private static HttpClient CreateHttpClient(Func<HttpRequestMessage, Task<HttpResponseMessage>> responder)
    {
        return new HttpClient(new StubHttpMessageHandler(responder))
        {
            BaseAddress = new Uri("https://example.test")
        };
    }

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> responder) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => responder(request);
    }

    private sealed class FixedHttpClientFactory(HttpClient client) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => client;
    }
}
