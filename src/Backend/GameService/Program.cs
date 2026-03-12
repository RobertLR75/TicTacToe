using FastEndpoints;
using FastEndpoints.Swagger;
using GameService.Configuration;
using GameService.Endpoints.Games.Create;
using GameService.Endpoints.Games.List;
using GameService.Endpoints.Games.UpdateStatus;
using GameService.Models;
using GameService.Persistence;
using GameService.Services;
using SharedLibrary.PostgreSql.EntityFramework;
using TicTacToe.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddGamePersistence(builder.Configuration);
builder.Services.AddGameEventPublishing(builder.Configuration);

builder.Services.AddScoped<IPostgresSqlStorageService<Game>, GameStorageService>();
builder.Services.AddScoped<IRequestHandler<UpdateGameStatusCommand, GameStatusUpdateResult>, UpdateGameStatusHandler>();
builder.Services.AddScoped<IUpdateUpdateGameStatusCommandHandler, ValidateGameStatusCommand.ValidateGameStatusCommandHandler>();
builder.Services.AddScoped<IRequestHandler<CreateGameCommand, Game>, CreateGameHandler>();
builder.Services.AddScoped<IRequestHandler<ListGamesQuery, IEnumerable<Game>>, ListGamesQueryHandler>();
builder.Services.AddScoped<IGameEventPublisher, MassTransitGameEventPublisher>();

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
