using FastEndpoints;
using FastEndpoints.Swagger;
using SharedLibrary.Services.Interfaces;
using TicTacToe.ServiceDefaults;
using UserService.Configuration;
using UserService.Features.Users.Endpoints.Create;
using UserService.Features.Users.Endpoints.Get;
using UserService.Features.Users.Endpoints.List;
using UserService.Features.Users.Endpoints.Update;
using UserService.Features.Users.Endpoints.UpdateStatus;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddUserCaching(builder.Configuration);
builder.Services.AddUserStorage(builder.Configuration);
builder.Services.AddUserEventPublishing(builder.Configuration);

builder.Services.AddScoped<ICreateUserHandler, CreateUserHandler>();
builder.Services.AddScoped<IUpdateUserHandler, UpdateUserHandler>();
builder.Services.AddScoped<IUpdateUserStatusHandler, UpdateUserStatusHandler>();
builder.Services.AddScoped<IGetUserHandler, GetUserHandler>();
builder.Services.AddScoped<IListUsersHandler, ListUsersHandler>();
builder.Services.AddScoped<IUserCacheService, UserCacheService>();
builder.Services.AddScoped<IUserEventPublisher, MassTransitUserEventPublisher>();

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

var app = builder.Build();

app.UseFastEndpoints();
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
{
    app.UseSwaggerGen();
}

app.Run();

namespace UserService
{
    public partial class Program;
}
