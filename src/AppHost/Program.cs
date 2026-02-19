var builder = DistributedApplication.CreateBuilder(args);

// Backend API - run as executable to avoid runtime issues
var backendPath = Path.Combine("..", "Backend", "GameService");
var gameService = builder.AddExecutable("gameservice", "dotnet", backendPath, "run", "--no-build")
    .WithHttpsEndpoint(port: 7082, name: "https")
    .WithHttpEndpoint(port: 5082, name: "http")
    .WithEnvironment("ASPNETCORE_URLS", "https://localhost:7082;http://localhost:5082");

// Frontend Blazor WASM - run as executable
var frontendPath = Path.Combine("..", "FrontEnd", "TicTacToeUI");
builder.AddExecutable("frontend", "dotnet", frontendPath, "run", "--no-build")
    .WithHttpsEndpoint(port: 7080, name: "https")
    .WithHttpEndpoint(port: 5080, name: "http")
    .WithEnvironment("ASPNETCORE_URLS", "https://localhost:7080;http://localhost:5080");

builder.Build().Run();

