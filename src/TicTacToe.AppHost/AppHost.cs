var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure resources
var redis = builder.AddRedis("redis");

var postgres = builder.AddPostgres("postgres")
    .AddDatabase("postgres-db");

var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);

var rabbitmq = builder.AddRabbitMQ("messaging", username, password)
    .WithManagementPlugin(); 

// Game state service - orchestrated by Aspire with explicit resource dependencies
builder.AddProject<Projects.GameStateService>("gamestateservice")
    .WithHttpEndpoint(port: 5110, name: "gamestateservice-http")
    .WithReference(redis)
    .WithReference(postgres)
    .WithReference(rabbitmq);

// Game Service - lobby/matchmaking service backed by PostgreSQL
var gameService = builder.AddProject<Projects.GameService>("gameservice")
    .WithReference(postgres)
    .WithReference(rabbitmq)
    .WithEnvironment("Messaging__EnableEventPublishing", "true")
    .WaitFor(postgres)
    .WaitFor(rabbitmq)
    .WithHttpEndpoint(port: 5120, name: "gameservice-http");

// Notification Service - stub API for future notification delivery
var gameNotificationService = builder.AddProject<Projects.GameNotificationService>("gamenotificationservice")
    .WithHttpEndpoint(port: 5130, name: "gamenotificationservice-http")
    .WithReference(postgres)
    .WithReference(rabbitmq)
    .WaitFor(postgres)
    .WaitFor(rabbitmq);

// Frontend - managed by Aspire, references gameservice for service discovery
builder.AddProject<Projects.TicTacToeMud>("frontend")
    .WithReference(gameService)
    .WithReference(gameNotificationService)
    .WithExternalHttpEndpoints();

builder.Build().Run();
