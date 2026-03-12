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

// Add GameApiClient typed HttpClient. Use Aspire-provided GameService URLs when available,
// otherwise fall back to the local GameService development endpoint.
builder.Services.AddHttpClient<GameApiClient>(client =>
{
    client.BaseAddress = new Uri(gameServiceBaseUrl);
});

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

app.MapGet("/logout", (HttpContext context) =>
{
    SessionUserStore.Clear(context.Session);
    return Results.Redirect("/login");
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static string RenderLoginPage(string? errorMessage = null)
{
    var encodedError = string.IsNullOrWhiteSpace(errorMessage)
        ? string.Empty
        : $"<p class=\"login-error\" role=\"alert\">{HtmlEncoder.Default.Encode(errorMessage)}</p>";

    return "<!doctype html>"
        + "<html lang=\"en\">"
        + "<head>"
        + "<meta charset=\"utf-8\">"
        + "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">"
        + "<title>Login</title>"
        + "<link rel=\"preconnect\" href=\"https://fonts.googleapis.com\">"
        + "<link rel=\"preconnect\" href=\"https://fonts.gstatic.com\" crossorigin>"
        + "<link href=\"https://fonts.googleapis.com/css2?family=Outfit:wght@400;600;700&display=swap\" rel=\"stylesheet\">"
        + "<link rel=\"stylesheet\" href=\"/_content/MudBlazor/MudBlazor.min.css\">"
        + "<style>"
        + ":root { --login-primary:#7e6fff; --login-surface:#0f1118; --login-bg-1:#000000; --login-bg-2:#131722; --login-text:#f5f7ff; --login-muted:#b7bfd6; --login-border:#2f3445; --login-input-bg:#121622; --login-input-border:#3b4460; --login-error:#ff8a8a; --login-error-bg:#3a1515; }"
        + "* { box-sizing: border-box; }"
        + "body { margin:0; font-family:'Outfit', 'Segoe UI', Tahoma, sans-serif; min-height:100vh; color:var(--login-text); background: radial-gradient(circle at top left, var(--login-bg-2), transparent 45%), linear-gradient(135deg, var(--login-bg-1), #07080c 65%); }"
        + ".login-page { min-height:100vh; display:grid; place-items:center; padding:1rem; }"
        + ".login-panel { width:min(100%, 28rem); border:1px solid var(--login-border); border-radius:1rem; background:var(--login-surface); box-shadow:0 22px 44px rgba(0,0,0,.55); padding:1.25rem; }"
        + ".login-title { margin:0; font-size:clamp(1.5rem, 4vw, 2rem); font-weight:700; letter-spacing:.02em; }"
        + ".login-subtitle { margin:.35rem 0 1rem; color:var(--login-muted); font-size:1rem; }"
        + ".login-form { display:grid; gap:.8rem; }"
        + ".login-label { font-weight:600; font-size:.95rem; }"
        + ".login-input { width:100%; border:1px solid var(--login-input-border); border-radius:.65rem; padding:.7rem .8rem; font-size:1rem; font-family:inherit; background:var(--login-input-bg); color:var(--login-text); }"
        + ".login-input::placeholder { color:var(--login-muted); }"
        + ".login-input:focus-visible { outline:3px solid color-mix(in srgb, var(--login-primary) 45%, white); outline-offset:2px; border-color:var(--login-primary); }"
        + ".login-button { display:inline-flex; justify-content:center; align-items:center; border:none; border-radius:.7rem; padding:.72rem .95rem; font-size:1rem; font-weight:600; color:#fff; background:var(--login-primary); cursor:pointer; transition:transform .15s ease, box-shadow .15s ease; }"
        + ".login-button:hover { transform:translateY(-1px); box-shadow:0 10px 18px rgba(126,111,255,.35); }"
        + ".login-button:focus-visible { outline:3px solid color-mix(in srgb, var(--login-primary) 35%, white); outline-offset:2px; }"
        + ".login-button:active { transform:translateY(0); box-shadow:none; }"
        + ".login-error { margin:0; color:var(--login-error); background:var(--login-error-bg); border:1px solid #7a2626; border-radius:.65rem; padding:.6rem .7rem; }"
        + "@media (max-width: 640px) { .login-panel { padding:1rem; border-radius:.9rem; } .login-subtitle { font-size:.95rem; } }"
        + "</style>"
        + "</head>"
        + "<body>"
        + "<main class=\"login-page\" data-testid=\"login-page\">"
        + "<section class=\"login-panel mud-paper mud-elevation-1\" aria-labelledby=\"login-title\">"
        + "<h1 id=\"login-title\" class=\"login-title\">Welcome Back</h1>"
        + "<p class=\"login-subtitle\">Sign in to continue to TicTacToe.</p>"
        + encodedError
        + "<form class=\"login-form\" method=\"post\" action=\"/login\">"
        + "<label class=\"login-label\" for=\"name\">Username</label>"
        + "<input class=\"login-input\" id=\"name\" name=\"name\" autocomplete=\"username\" required>"
        + "<button class=\"login-button mud-button-root mud-button-filled mud-button-filled-primary\" type=\"submit\">Continue</button>"
        + "</form>"
        + "</section>"
        + "</main>"
        + "</body>"
        + "</html>";
}

public partial class Program;
