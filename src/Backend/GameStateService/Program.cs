using FastEndpoints;
using FastEndpoints.Swagger;
using GameStateService.Configuration;
using GameStateService.Consumers;
using GameStateService.Features.GameStates.Endpoints.Get;
using GameStateService.Features.GameStates.Endpoints.Update;
using GameStateService.Features.GameStates.Entities;
using GameStateService.Services;
using SharedLibrary.Interfaces;
using SharedLibrary.Redis;
using SharedLibrary.Services.Interfaces;
using TicTacToe.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.ConfigureRedisDistributedCache("gamestateservice");

builder.Services.AddGameEventPublishing(builder.Configuration);

builder.Services.AddScoped<IPersistenceService<GameEntity>, GameRedisPersistenceService>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameInitializedPublisher, GameInitializedPublisher>();
builder.Services.AddScoped<IGameStateUpdatedEventPublisher, GameStateUpdatedEventPublisher>();
builder.Services.AddScoped<IRequestHandler<ApplyMove, GameLogicMoveResult>, GameStateHandler>();
builder.Services.AddScoped<IRequestHandler<InitializeGame, GameEntity>, InitializeGame.InitializeGameHandler>();
builder.Services.AddScoped<IGetGameHandler, GetGameHandler>();
builder.Services.AddScoped<IUpdateGameStateHandler, UpdateGameStateHandler>();
builder.Services.AddScoped<ICheckStateService, CheckStateService>();

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorUI", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5081",
                "https://localhost:5081",
                "http://localhost:5080",
                "https://localhost:5080",
                "http://localhost:5088",
                "https://localhost:5088",
                "https://localhost:7293"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Ensure CORS middleware runs before FastEndpoints so endpoints include CORS headers
app.UseCors("AllowBlazorUI");

app.UseFastEndpoints();
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.Run();

namespace GameStateService
{
    public partial class Program;
}
