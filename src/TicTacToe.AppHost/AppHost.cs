using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure resources
var redis = builder.AddRedis("redis");

var mongo = builder.AddMongoDB("mongodb")
    .AddDatabase("mongodb-db");

var postgres = builder.AddPostgres("postgres")
    .AddDatabase("postgres-db");

var rabbitmq = builder.AddRabbitMQ("rabbitmq");

// Backend API - managed by Aspire via AddProject, explicitly declare endpoints with unique names
var gameservice = builder.AddProject<Projects.GameStateService>("gamestateservice")
    .WithHttpEndpoint(port: 5110, name: "gamestateservice-http")
    .WithReference(redis)
    .WithReference(mongo)
    .WithReference(postgres)
    .WithReference(rabbitmq);

// Game Service - lobby/matchmaking service backed by Redis
var gameService = builder.AddProject<Projects.GameService>("gameservice")
    .WithReference(redis)
    .WaitFor(redis)
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
    .WithReference(gameservice)
    .WithReference(gameNotificationService)
    .WithExternalHttpEndpoints();

builder.Build().Run();
