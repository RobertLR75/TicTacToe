using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TicTacToeMud.Tests;

public class SessionLoginFlowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SessionLoginFlowTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Home_redirects_to_login_when_session_user_is_missing()
    {
        using var client = CreateClient(allowAutoRedirect: false);

        using var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/login", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task Home_is_served_after_successful_login_creates_session_identity()
    {
        using var client = CreateClient(allowAutoRedirect: false);

        using var loginResponse = await client.PostAsync("/login", BuildLoginContent("Alice"));

        Assert.Equal(HttpStatusCode.Redirect, loginResponse.StatusCode);
        Assert.Equal("/", loginResponse.Headers.Location?.OriginalString);

        using var homeResponse = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, homeResponse.StatusCode);
    }

    [Fact]
    public async Task Login_rejects_whitespace_username_and_does_not_authenticate_session()
    {
        using var client = CreateClient(allowAutoRedirect: false);

        using var loginResponse = await client.PostAsync("/login", BuildLoginContent("   "));

        Assert.Equal(HttpStatusCode.BadRequest, loginResponse.StatusCode);
        var content = await loginResponse.Content.ReadAsStringAsync();
        Assert.Contains("Username is required.", content);

        using var homeResponse = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.Redirect, homeResponse.StatusCode);
        Assert.Equal("/login", homeResponse.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task Login_get_redirects_authenticated_user_to_home()
    {
        using var client = CreateClient(allowAutoRedirect: false);

        using var loginPostResponse = await client.PostAsync("/login", BuildLoginContent("Bob"));
        Assert.Equal(HttpStatusCode.Redirect, loginPostResponse.StatusCode);

        using var loginGetResponse = await client.GetAsync("/login");

        Assert.Equal(HttpStatusCode.Redirect, loginGetResponse.StatusCode);
        Assert.Equal("/", loginGetResponse.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task Game_page_renders_game_list_shell_with_new_game_action()
    {
        using var client = CreateClient(allowAutoRedirect: false);

        using var loginResponse = await client.PostAsync("/login", BuildLoginContent("Charlie"));
        Assert.Equal(HttpStatusCode.Redirect, loginResponse.StatusCode);

        using var gameResponse = await client.GetAsync("/game");

        Assert.Equal(HttpStatusCode.OK, gameResponse.StatusCode);
        var content = await gameResponse.Content.ReadAsStringAsync();
        Assert.Contains("Created Games", content);
        Assert.Contains("New Game", content);
    }

    private HttpClient CreateClient(bool allowAutoRedirect)
    {
        return _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = allowAutoRedirect
        });
    }

    private static FormUrlEncodedContent BuildLoginContent(string username)
    {
        return new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["name"] = username
        });
    }
}
