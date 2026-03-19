using GameStateService.Services;
using GameStateService.Features.GameStates.Endpoints.Update;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharedLibrary.Interfaces;
using SharedLibrary.Services.Interfaces;
using GameStateService.Features.GameStates.Entities;

namespace GameStateService.Tests.Testing;

public sealed class GameStateServiceWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _rabbitMqConnectionString;
    private readonly IGameRepository? _repositoryOverride;
    private readonly IPersistenceService<GameEntity>? _persistenceOverride;
    private readonly bool _enableEventPublishing;

    public GameStateServiceWebApplicationFactory(
        string rabbitMqConnectionString,
        IGameRepository? repositoryOverride = null,
        IPersistenceService<GameEntity>? persistenceOverride = null,
        bool enableEventPublishing = true)
    {
        _rabbitMqConnectionString = rabbitMqConnectionString;
        _repositoryOverride = repositoryOverride;
        _persistenceOverride = persistenceOverride;
        _enableEventPublishing = enableEventPublishing;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var rabbitMqUri = new Uri(_rabbitMqConnectionString);
        var credentials = rabbitMqUri.UserInfo.Split(':', 2);
        var username = credentials.Length > 0 && !string.IsNullOrWhiteSpace(credentials[0]) ? credentials[0] : "guest";
        var password = credentials.Length > 1 && !string.IsNullOrWhiteSpace(credentials[1]) ? credentials[1] : "guest";

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Messaging:EnableEventPublishing"] = _enableEventPublishing.ToString(),
                ["Messaging:RabbitMq:Host"] = rabbitMqUri.Host,
                ["Messaging:RabbitMq:Port"] = rabbitMqUri.Port.ToString(),
                ["Messaging:RabbitMq:Username"] = username,
                ["Messaging:RabbitMq:Password"] = password,
                ["ConnectionStrings:rabbitmq"] = _rabbitMqConnectionString
            });
        });

        if (_repositoryOverride is null && _persistenceOverride is null)
            return;

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IGameInitializedPublisher>();
            services.RemoveAll<IGameStateUpdatedEventPublisher>();
            services.AddSingleton<IGameInitializedPublisher, NoOpGameInitializedPublisher>();
            services.AddSingleton<IGameStateUpdatedEventPublisher, NoOpGameStateUpdatedEventPublisher>();

            if (_persistenceOverride is not null)
            {
                services.RemoveAll<IPersistenceService<GameEntity>>();
                services.AddSingleton(_persistenceOverride);
            }

            if (_repositoryOverride is not null)
            {
                services.RemoveAll<IGameRepository>();
                services.AddSingleton(_repositoryOverride);
            }
        });
    }

    private sealed class NoOpGameInitializedPublisher : IGameInitializedPublisher
    {
        public Task PublishAsync(GameStateService.Consumers.GameInitializedEvent ev, CancellationToken cancellation = default)
            => Task.CompletedTask;
    }

    private sealed class NoOpGameStateUpdatedEventPublisher : IGameStateUpdatedEventPublisher
    {
        public Task PublishAsync(GameStateService.Features.GameStates.Endpoints.Update.GameStateUpdatedEvent ev, CancellationToken cancellation = default)
            => Task.CompletedTask;
    }
}
