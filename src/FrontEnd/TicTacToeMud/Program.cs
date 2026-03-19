using MudBlazor.Services;
using Service.Contracts.Services;
using TicTacToeMud.Components;
using TicTacToeMud.Session;
using TicTacToeMud.Services;
using TicTacToe.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add MudBlazor services
builder.Services.AddMudServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "TicTacToeMud.Session";
});

var gameServiceBaseUrl = builder.Configuration.GetValue<string>("Services:gameservice:https:0")
    ?? builder.Configuration.GetValue<string>("Services:gameservice:http:0")
    ?? "https://localhost:7022";

var gameStateServiceBaseUrl = builder.Configuration.GetValue<string>("Services:gamestateservice:https:0")
    ?? builder.Configuration.GetValue<string>("Services:gamestateservice:http:0")
    ?? "https://localhost:7110";

builder.Services.AddHttpClient("api/games", client =>
{
    client.BaseAddress = new Uri(gameServiceBaseUrl);
});
builder.Services.AddScoped<GameApiClient>();

builder.Services.AddHttpClient<GameStateServiceClient>(client =>
{
    client.BaseAddress = new Uri(gameStateServiceBaseUrl);
});

// Add SignalR game hub client (GameNotificationService)
builder.Services.AddScoped<GameHubClient>();
builder.Services.AddScoped<INotificationService, MudNotificationService>();

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
    if (HttpMethods.IsGet(context.Request.Method))
    {
        var isAuthenticated = SessionUserStore.TryRead(context, out _);

        if (context.Request.Path == "/" && !isAuthenticated)
        {
            context.Response.Redirect("/login");
            return;
        }

        if (context.Request.Path == "/login" && isAuthenticated)
        {
            context.Response.Redirect("/");
            return;
        }
    }

    await next();
});

app.UseAntiforgery();

app.MapPost("/login/submit", async (HttpContext context) =>
{
    if (SessionUserStore.TryRead(context, out _))
    {
        return Results.Redirect("/");
    }

    var form = await context.Request.ReadFormAsync();
    var username = form["name"].ToString().Trim();

    if (string.IsNullOrWhiteSpace(username))
    {
        var encodedError = Uri.EscapeDataString("Username is required.");
        return Results.Redirect($"/login?error={encodedError}");
    }

    SessionUserStore.Write(context.Session, new SessionUser(Guid.NewGuid(), username));
    return Results.Redirect("/");
})
.DisableAntiforgery();

app.MapGet("/logout", (HttpContext context) =>
{
    SessionUserStore.Clear(context.Session);
    return Results.Redirect("/login");
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


public partial class Program;
