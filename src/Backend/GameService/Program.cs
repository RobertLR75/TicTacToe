using FastEndpoints;
using GameService.Configuration;
using GameService.Endpoints.Games.Create;
using GameService.Endpoints.Games.List;
using GameService.Endpoints.Games.UpdateStatus;
using GameService.Models;
using GameService.Persistence;
using GameService.Services;
using SharedLibrary.FastEndpoints;
using SharedLibrary.PostgreSql.EntityFramework;
using TicTacToe.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddGamePersistence(builder.Configuration);
builder.Services.AddGameEventPublishing(builder.Configuration);

var postgresConnectionString = builder.Configuration.GetConnectionString("postgres");
if (string.IsNullOrWhiteSpace(postgresConnectionString))
{
    throw new InvalidOperationException("ConnectionStrings:postgres is required for GameService.");
}

builder.Services.AddScoped<IPostgresSqlStorageService<Game>, GameStorageService>();
builder.Services.AddScoped<IRequestHandler<UpdateGameStatusCommand, GameStatusUpdateResult>, UpdateGameStatusHandler>();
builder.Services.AddScoped<IUpdateUpdateGameStatusCommandHandler, ValidateGameStatusCommand.ValidateGameStatusCommandHandler>();
builder.Services.AddScoped<IRequestHandler<CreateGameCommand, Game>, CreateGameHandler>();
builder.Services.AddScoped<IRequestHandler<ListGamesQuery, IEnumerable<Game>>, ListGamesQueryHandler>();
builder.Services.AddScoped<IGameEventPublisher, MassTransitGameEventPublisher>();

builder.ConfigureFastEndPoints();


var app = builder.Build();

app.Services.ApplyGameMigrations();

app.UseFastEndpoints();
app.MapDefaultEndpoints();

app.Run();

namespace GameService
{
    public partial class Program;
}
