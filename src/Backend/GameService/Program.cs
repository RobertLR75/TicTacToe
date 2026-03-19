using FastEndpoints;
using FastEndpoints.Swagger;
using GameService.Configuration;
using GameService.Features.Games.Endpoints.Create;
using GameService.Features.Games.Endpoints.Get;
using GameService.Features.Games.Endpoints.List;
using GameService.Features.Games.Endpoints.UpdateStatus;
using GameService.Persistence;
using GameService.Services;
using SharedLibrary.PostgreSql.EntityFramework;
using TicTacToe.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddGamePersistence(builder.Configuration);
builder.Services.AddGameEventPublishing(builder.Configuration);

builder.Services.AddScoped<IGameStorageService, GameStorageService>();
builder.Services.AddScoped<IUpdateGameStatusHandler, UpdateGameStatusHandler>();
builder.Services.AddScoped<IUpdateUpdateGameStatusCommandHandler, ValidateGameStatusCommand.ValidateGameStatusCommandHandler>();
builder.Services.AddScoped<ICreateGameHandler, CreateGameHandler>();
builder.Services.AddScoped<IGetGameHandler, GetGameHandler>();
builder.Services.AddScoped<IListGamesHandler, ListGamesHandler>();
builder.Services.AddScoped<ICreateGameEventPublisher, CreateGameEventPublisher>();
builder.Services.AddScoped<IUpdateGameStatusEventPublisher, UpdateGameStatusEventPublisher>();
// builder.Services.AddHttpClient<IGameStateReadClient, GameStateReadClient>(client =>
// {
//     var gameStateServiceBaseUrl = builder.Configuration.GetValue<string>("Services:gamestateservice:https:0")
//         ?? builder.Configuration.GetValue<string>("Services:gamestateservice:http:0")
//         ?? "https://localhost:7110";
//
//     client.BaseAddress = new Uri(gameStateServiceBaseUrl);
// });

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();


var app = builder.Build();

await app.Services.EnsureGamePersistenceReadyBeforeStartupAsync(app.Logger, app.Lifetime.ApplicationStopping);

app.UseFastEndpoints();
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
{
    app.UseSwaggerGen();
}

app.Run();

namespace GameService
{
    public partial class Program;
}
