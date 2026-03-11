using FastEndpoints;
using FastEndpoints.Swagger;
using GameStateService.Configuration;
using GameStateService.Consumers;
using GameStateService.Endpoints.Games.Get;
using GameStateService.Endpoints.Games.MakeMove;
using GameStateService.GameState;
using GameStateService.Services;
using TicTacToe.ServiceDefaults;
using GameState = GameStateService.GameState.GameState;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGameEventPublishing(builder.Configuration);

builder.Services.AddSingleton<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameEventPublisher, MassTransitGameStateEventPublisher>();
builder.Services.AddScoped<IRequestHandler<CheckWinner, CheckWinnerResult>, CheckWinnerHandler>();
builder.Services.AddScoped<IRequestHandler<CheckDraw, CheckDrawResult>, CheckDrawHandler>();
builder.Services.AddScoped<IRequestHandler<GameState, GameLogicMoveResult>, GameStateHandler>();
builder.Services.AddScoped<IRequestHandler<InitializeGame, GameStateService.Models.GameState>, InitializeGame.InitializeGameHandler>();
builder.Services.AddScoped<IRequestHandler<GetGame, GetGameQueryResult>, GetGame.GetGameHandler>();
builder.Services.AddScoped<IRequestHandler<MakeMove, MakeMoveCommandResult>, MakeMoveHandler>();

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

public partial class Program;
