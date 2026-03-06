using FastEndpoints;
using GameService.Models;
using GameService.Persistence;
using GameService.Services;
using SharedLibrary.FastEndpoints;
using SharedLibrary.PostgreSql.EntityFramework;
using TicTacToe.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddGamePersistence(builder.Configuration);

var postgresConnectionString = builder.Configuration.GetConnectionString("postgres");
if (string.IsNullOrWhiteSpace(postgresConnectionString))
{
    throw new InvalidOperationException("ConnectionStrings:postgres is required for GameService.");
}

builder.Services.AddScoped<IPostgresSqlStorageService<GameModel>, GameStorageService>();
builder.Services.AddScoped<IUpdateGameStatusCommandHandler, UpdateGameStatusCommand.UpdateUpdateGameStatusCommandHandler>();
builder.Services.AddScoped<IRequestHandler<UpdateGameStatusCommand, GameStatusUpdateResult>, UpdateGameStatusCommand.UpdateUpdateGameStatusCommandHandler>();
builder.Services.AddScoped<IUpdateUpdateGameStatusCommandHandler, ValidateGameStatusCommand.ValidateGameStatusCommandHandler>();

builder.ConfigureFastEndPoints();


var app = builder.Build();

app.Services.ApplyGameMigrations();

app.UseFastEndpoints();
app.MapDefaultEndpoints();

app.Run();

public partial class Program;
