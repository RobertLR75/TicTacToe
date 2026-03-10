using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using GameStateService.Contracts.Events;
using GameStateService.Endpoints.Games.Create;
using GameStateService.Endpoints.Games.MakeMove;
using GameStateService.Models;
using GameStateService.Services;
using GameStateService.Tests.Testing;
using MassTransit;
using Testcontainers.RabbitMq;
using Xunit;

namespace GameStateService.Tests;

[Collection(RabbitMqCollection.Name)]
public sealed class EventPublishingIntegrationTests : IntegrationTestBase
{
    private readonly RabbitMqFixture _fixture;

    public EventPublishingIntegrationTests(RabbitMqFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Create_endpoint_publishes_initialized_event_on_success()
    {
        var initializedEvents = new ConcurrentBag<GameStateInitializedEvent>();
        var rabbit = ParseRabbitConnectionString(_fixture.ConnectionString);
        var queueName = $"test-gs-initialized-{Guid.NewGuid():N}";

        var listener = BuildRabbitMqListener(queueName, rabbit.Uri, rabbit.Username, rabbit.Password, e =>
        {
            e.Handler<GameStateInitializedEvent>(context =>
            {
                initializedEvents.Add(context.Message);
                return Task.CompletedTask;
            });
        });

        await listener.StartAsync();

        try
        {
            using var factory = new GameStateServiceWebApplicationFactory(_fixture.ConnectionString);
            using var client = factory.CreateClient();

            var response = await client.PostAsync("/api/games", JsonContent.Create(new { }));

            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);

            var published = await WaitForAsync(() => Task.FromResult(initializedEvents.Count == 1), TimeSpan.FromSeconds(10));

            Assert.True(published);
        }
        finally
        {
            await listener.StopAsync();
        }
    }

    [Fact]
    public async Task Create_endpoint_does_not_publish_initialized_event_when_create_fails()
    {
        var initializedEvents = new ConcurrentBag<GameStateInitializedEvent>();
        var rabbit = ParseRabbitConnectionString(_fixture.ConnectionString);
        var queueName = $"test-gs-initialized-failed-{Guid.NewGuid():N}";

        var listener = BuildRabbitMqListener(queueName, rabbit.Uri, rabbit.Username, rabbit.Password, e =>
        {
            e.Handler<GameStateInitializedEvent>(context =>
            {
                initializedEvents.Add(context.Message);
                return Task.CompletedTask;
            });
        });

        await listener.StartAsync();

        try
        {
            using var factory = new GameStateServiceWebApplicationFactory(_fixture.ConnectionString, new ThrowOnCreateRepository());
            using var client = factory.CreateClient();

            var response = await client.PostAsync("/api/games", JsonContent.Create(new { }));

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var published = await WaitForAsync(() => Task.FromResult(initializedEvents.Count > 0), TimeSpan.FromSeconds(3));

            Assert.False(published);
        }
        finally
        {
            await listener.StopAsync();
        }
    }

    [Fact]
    public async Task MakeMove_endpoint_publishes_updated_event_on_successful_update()
    {
        var updatedEvents = new ConcurrentBag<GameStateUpdatedEvent>();
        var rabbit = ParseRabbitConnectionString(_fixture.ConnectionString);
        var queueName = $"test-gs-updated-{Guid.NewGuid():N}";

        var listener = BuildRabbitMqListener(queueName, rabbit.Uri, rabbit.Username, rabbit.Password, e =>
        {
            e.Handler<GameStateUpdatedEvent>(context =>
            {
                updatedEvents.Add(context.Message);
                return Task.CompletedTask;
            });
        });

        await listener.StartAsync();

        try
        {
            using var factory = new GameStateServiceWebApplicationFactory(_fixture.ConnectionString);
            using var client = factory.CreateClient();

            var createResponse = await client.PostAsync("/api/games", JsonContent.Create(new { }));
            var created = await createResponse.Content.ReadFromJsonAsync<CreateGameResponse>();
            Assert.NotNull(created);

            var moveResponse = await client.PostAsJsonAsync($"/api/games/{created!.GameId}/moves", new MakeMoveRequest
            {
                GameId = created.GameId,
                Row = 0,
                Col = 0
            });

            Assert.Equal(HttpStatusCode.Accepted, moveResponse.StatusCode);

            var published = await WaitForAsync(() => Task.FromResult(updatedEvents.Count == 1), TimeSpan.FromSeconds(10));

            Assert.True(published);
            Assert.Equal(created.GameId, updatedEvents.Single().GameId);
        }
        finally
        {
            await listener.StopAsync();
        }
    }

    [Fact]
    public async Task MakeMove_endpoint_does_not_publish_updated_event_when_game_is_missing()
    {
        var updatedEvents = new ConcurrentBag<GameStateUpdatedEvent>();
        var rabbit = ParseRabbitConnectionString(_fixture.ConnectionString);
        var queueName = $"test-gs-updated-missing-{Guid.NewGuid():N}";

        var listener = BuildRabbitMqListener(queueName, rabbit.Uri, rabbit.Username, rabbit.Password, e =>
        {
            e.Handler<GameStateUpdatedEvent>(context =>
            {
                updatedEvents.Add(context.Message);
                return Task.CompletedTask;
            });
        });

        await listener.StartAsync();

        try
        {
            using var factory = new GameStateServiceWebApplicationFactory(_fixture.ConnectionString);
            using var client = factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/games/missing/moves", new MakeMoveRequest
            {
                GameId = "missing",
                Row = 0,
                Col = 0
            });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var published = await WaitForAsync(() => Task.FromResult(updatedEvents.Count > 0), TimeSpan.FromSeconds(3));

            Assert.False(published);
        }
        finally
        {
            await listener.StopAsync();
        }
    }

    private static (Uri Uri, string Username, string Password) ParseRabbitConnectionString(string connectionString)
    {
        var rabbitUri = new Uri(connectionString);
        var credentials = rabbitUri.UserInfo.Split(':', 2);
        var username = credentials.Length > 0 && !string.IsNullOrWhiteSpace(credentials[0]) ? credentials[0] : "guest";
        var password = credentials.Length > 1 && !string.IsNullOrWhiteSpace(credentials[1]) ? credentials[1] : "guest";
        var hostUri = new Uri($"rabbitmq://{rabbitUri.Host}:{rabbitUri.Port}/");
        return (hostUri, username, password);
    }

    private sealed class ThrowOnCreateRepository : IGameRepository
    {
        public Models.GameState CreateGame() => throw new InvalidOperationException("create failed");

        public Models.GameState? GetGame(string gameId) => null;

        public void UpdateGame(Models.GameState game)
        {
        }

        public void DeleteGame(string gameId)
        {
        }
    }
}
