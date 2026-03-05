using MudBlazor.Services;
using TicTacToeMud.Components;
using TicTacToeMud.Services;
using TicTacToe.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add MudBlazor services
builder.Services.AddMudServices();

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


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
