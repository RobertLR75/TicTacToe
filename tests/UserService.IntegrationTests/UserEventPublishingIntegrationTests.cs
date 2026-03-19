using System.Collections.Concurrent;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Options;
using Service.Contracts.Events;
using UserService.Configuration;
using UserService.Features.Users.Endpoints.Create;
using UserService.Features.Users.Endpoints.Update;
using UserService.Features.Users.Entities;
using UserService.IntegrationTests.Testing;
using UserService.Services;
using Xunit;

namespace UserService.IntegrationTests;

[Collection(RabbitMqCollection.Name)]
public sealed class UserEventPublishingIntegrationTests(RabbitMqFixture fixture)
{
    [Fact]
    public async Task UserCreatedEventHandler_publishes_event_when_enabled()
    {
        var events = new ConcurrentBag<UserCreated>();
        var rabbit = ParseRabbitConnectionString(fixture.ConnectionString);
        var listener = BuildRabbitMqListener($"test-user-created-{Guid.NewGuid():N}", rabbit.Uri, rabbit.Username, rabbit.Password, endpoint =>
        {
            endpoint.Handler<UserCreated>(context =>
            {
                events.Add(context.Message);
                return Task.CompletedTask;
            });
        });

        await listener.StartAsync();

        try
        {
            var publisher = new MassTransitUserEventPublisher(listener, Options.Create(new MessagingOptions { EnableEventPublishing = true }));
            var sut = new UserCreatedEvent.UserCreatedEventHandler(publisher);

            await sut.HandleAsync(new UserCreatedEvent { User = new UserEntity { Id = Guid.NewGuid(), Name = "Alice", CreatedAt = DateTimeOffset.UtcNow } }, CancellationToken.None);

            Assert.True(await WaitForAsync(() => Task.FromResult(events.Count == 1), TimeSpan.FromSeconds(10)));
        }
        finally
        {
            await listener.StopAsync();
        }
    }

    [Fact]
    public async Task UserUpdatedEventHandler_publishes_event_when_enabled()
    {
        var events = new ConcurrentBag<UserUpdated>();
        var rabbit = ParseRabbitConnectionString(fixture.ConnectionString);
        var listener = BuildRabbitMqListener($"test-user-updated-{Guid.NewGuid():N}", rabbit.Uri, rabbit.Username, rabbit.Password, endpoint =>
        {
            endpoint.Handler<UserUpdated>(context =>
            {
                events.Add(context.Message);
                return Task.CompletedTask;
            });
        });

        await listener.StartAsync();

        try
        {
            var publisher = new MassTransitUserEventPublisher(listener, Options.Create(new MessagingOptions { EnableEventPublishing = true }));
            var sut = new UserUpdatedEvent.UserUpdatedEventHandler(publisher);

            await sut.HandleAsync(new UserUpdatedEvent { User = new UserEntity { Id = Guid.NewGuid(), Name = "Bob", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow } }, CancellationToken.None);

            Assert.True(await WaitForAsync(() => Task.FromResult(events.Count == 1), TimeSpan.FromSeconds(10)));
        }
        finally
        {
            await listener.StopAsync();
        }
    }

    private static async Task<bool> WaitForAsync(Func<Task<bool>> condition, TimeSpan timeout)
    {
        var started = DateTimeOffset.UtcNow;
        while (DateTimeOffset.UtcNow - started < timeout)
        {
            if (await condition())
            {
                return true;
            }

            await Task.Delay(200);
        }

        return false;
    }

    private static IBusControl BuildRabbitMqListener(string queueName, Uri rabbitMqUri, string username, string password, Action<IRabbitMqReceiveEndpointConfigurator> configureHandler)
    {
        return Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host(rabbitMqUri, h =>
            {
                h.Username(username);
                h.Password(password);
            });

            cfg.ReceiveEndpoint(queueName, configureHandler);
        });
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
}
