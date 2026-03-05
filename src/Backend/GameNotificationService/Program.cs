using FastEndpoints;
using FastEndpoints.Swagger;
using GameNotificationService.Configuration;
using GameNotificationService.Hubs;
using GameNotificationService.Persistence;
using GameNotificationService.Services;
using TicTacToe.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGameEventConsumers(builder.Configuration);
builder.Services.AddNotificationPersistence(builder.Configuration);
builder.Services.AddSingleton<IGameNotificationPublisher, SignalRGameNotificationPublisher>();
builder.Services.AddSignalR();

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

app.Services.ApplyNotificationMigrations();

app.UseCors("AllowBlazorUI");

app.UseFastEndpoints();
app.MapDefaultEndpoints();
app.MapHub<GameHub>("/hubs/game");

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.Run();
