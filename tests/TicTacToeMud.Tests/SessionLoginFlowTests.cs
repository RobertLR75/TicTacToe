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
    public async Task Login_get_renders_themed_login_markup_for_unauthenticated_user()
    {
        using var client = CreateClient(allowAutoRedirect: false);

        using var loginResponse = await client.GetAsync("/login");

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var content = await loginResponse.Content.ReadAsStringAsync();
        Assert.Contains("data-testid=\"login-page\"", content);
        Assert.Contains("Welcome Back", content);
        Assert.Contains("mud-paper", content);
        Assert.Contains("login-button", content);
        Assert.Contains("action=\"/login\"", content);
        Assert.Contains("--login-surface:#0f1118", content);
        Assert.Contains("--login-bg-1:#000000", content);
        Assert.Contains("--login-text:#f5f7ff", content);
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

    [Fact]
    public async Task Authenticated_pages_render_logout_action_in_top_app_bar()
    {
        using var client = CreateClient(allowAutoRedirect: false);

        using var loginResponse = await client.PostAsync("/login", BuildLoginContent("Dana"));
        Assert.Equal(HttpStatusCode.Redirect, loginResponse.StatusCode);

        using var homeResponse = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, homeResponse.StatusCode);
        var content = await homeResponse.Content.ReadAsStringAsync();
        Assert.Contains("Logout", content);
        Assert.Contains("/logout", content);
    }

    [Fact]
    public async Task Logout_clears_session_and_redirects_user_to_login()
    {
        using var client = CreateClient(allowAutoRedirect: false);

        using var loginResponse = await client.PostAsync("/login", BuildLoginContent("Eve"));
        Assert.Equal(HttpStatusCode.Redirect, loginResponse.StatusCode);

        using var logoutResponse = await client.GetAsync("/logout");

        Assert.Equal(HttpStatusCode.Redirect, logoutResponse.StatusCode);
        Assert.Equal("/login", logoutResponse.Headers.Location?.OriginalString);

        using var homeResponse = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.Redirect, homeResponse.StatusCode);
        Assert.Equal("/login", homeResponse.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task Game_page_renders_logout_action_in_shared_layout()
    {
        using var client = CreateClient(allowAutoRedirect: false);

        using var loginResponse = await client.PostAsync("/login", BuildLoginContent("Frank"));
        Assert.Equal(HttpStatusCode.Redirect, loginResponse.StatusCode);

        using var gameResponse = await client.GetAsync("/game");

        Assert.Equal(HttpStatusCode.OK, gameResponse.StatusCode);
        var content = await gameResponse.Content.ReadAsStringAsync();
        Assert.Contains("Logout", content);
        Assert.Contains("/logout", content);
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
