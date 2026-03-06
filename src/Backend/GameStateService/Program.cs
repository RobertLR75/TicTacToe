using FastEndpoints;
using FastEndpoints.Swagger;
using GameStateService.Configuration;
using GameStateService.Endpoints.Games.Create;
using GameStateService.Endpoints.Games.Get;
using GameStateService.Endpoints.Games.MakeMove;
using GameStateService.Services;
using TicTacToe.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGameEventPublishing(builder.Configuration);

builder.Services.AddSingleton<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameEventPublisher, MassTransitGameEventPublisher>();
builder.Services.AddScoped<IRequestHandler<CheckWinnerRequest, CheckWinnerResult>, CheckWinnerRequestHandler>();
builder.Services.AddScoped<IRequestHandler<CheckDrawRequest, CheckDrawResult>, CheckDrawRequestHandler>();
builder.Services.AddScoped<IRequestHandler<GameLogicMoveRequest, GameLogicMoveResult>, GameLogicMoveRequestHandler>();
builder.Services.AddScoped<IRequestHandler<CreateGameCommand, CreateGameResponse>, CreateGameCommandHandler>();
builder.Services.AddScoped<IRequestHandler<GetGameQuery, GetGameQueryResult>, GetGameQueryHandler>();
builder.Services.AddScoped<IRequestHandler<MakeMoveCommand, MakeMoveCommandResult>, MakeMoveCommandHandler>();

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
