using FastEndpoints;
using FastEndpoints.Swagger;
using GameService.Services;
using TicTacToe.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRedisClient("redis");

builder.Services.AddSingleton<GameRepository>();

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

var app = builder.Build();

app.UseFastEndpoints();
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.Run();
