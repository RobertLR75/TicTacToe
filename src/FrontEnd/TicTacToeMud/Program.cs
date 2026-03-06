using MudBlazor.Services;
using System.Text.Encodings.Web;
using TicTacToeMud.Components;
using TicTacToeMud.Session;
using TicTacToeMud.Services;
using TicTacToe.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add MudBlazor services
builder.Services.AddMudServices();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "TicTacToeMud.Session";
});

// Add GameApiClient typed HttpClient. Use local GameStateService in dev; Aspire dashboard will override when running under Aspire.
builder.Services.AddHttpClient<GameApiClient>(client =>
{
    // Local development: point directly to GameStateService
    client.BaseAddress = new Uri("https://localhost:7110");

    // When running under Aspire, the dashboard will inject service discovery and override this base address.
    // client.BaseAddress = new Uri("https+http://gamestateservice");
});

// Add SignalR game hub client (GameNotificationService)
builder.Services.AddScoped<GameHubClient>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();

app.UseSession();

app.Use(async (context, next) =>
{
    if (HttpMethods.IsGet(context.Request.Method)
        && context.Request.Path == "/"
        && !SessionUserStore.TryRead(context, out _))
    {
        context.Response.Redirect("/login");
        return;
    }

    await next();
});

app.UseAntiforgery();

app.MapGet("/login", (HttpContext context) =>
{
    if (SessionUserStore.TryRead(context, out _))
    {
        return Results.Redirect("/");
    }

    return Results.Content(RenderLoginPage(), "text/html");
});

app.MapPost("/login", async (HttpContext context) =>
{
    if (SessionUserStore.TryRead(context, out _))
    {
        return Results.Redirect("/");
    }

    var form = await context.Request.ReadFormAsync();
    var username = form["name"].ToString().Trim();

    if (string.IsNullOrWhiteSpace(username))
    {
        return Results.Content(RenderLoginPage("Username is required."), "text/html", statusCode: StatusCodes.Status400BadRequest);
    }

    SessionUserStore.Write(context.Session, new SessionUser(Guid.NewGuid(), username));
    return Results.Redirect("/");
})
.DisableAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static string RenderLoginPage(string? errorMessage = null)
{
    var encodedError = string.IsNullOrWhiteSpace(errorMessage)
        ? string.Empty
        : $"<p style=\"color:#b00020;margin-bottom:1rem;\">{HtmlEncoder.Default.Encode(errorMessage)}</p>";

    return "<!doctype html>"
        + "<html lang=\"en\">"
        + "<head>"
        + "<meta charset=\"utf-8\">"
        + "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">"
        + "<title>Login</title>"
        + "<style>"
        + "body { font-family: sans-serif; margin: 2rem; }"
        + "form { max-width: 24rem; display: grid; gap: 0.75rem; }"
        + "input { padding: 0.6rem; font-size: 1rem; }"
        + "button { padding: 0.6rem; font-size: 1rem; cursor: pointer; }"
        + "</style>"
        + "</head>"
        + "<body>"
        + "<h1>Login</h1>"
        + encodedError
        + "<form method=\"post\" action=\"/login\">"
        + "<label for=\"name\">Username</label>"
        + "<input id=\"name\" name=\"name\" autocomplete=\"username\" required>"
        + "<button type=\"submit\">Continue</button>"
        + "</form>"
        + "</body>"
        + "</html>";
}

public partial class Program;
