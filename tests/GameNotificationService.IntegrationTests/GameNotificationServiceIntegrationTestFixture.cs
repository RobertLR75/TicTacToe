using GameNotificationService.Configuration;
using GameNotificationService.Persistence;
using GameNotificationService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Service.Contracts.Notifications;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using TicTacToe.Testing;
using Xunit;

namespace GameNotificationService.IntegrationTests;

public sealed class GameNotificationServiceIntegrationTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder().Build();
    private readonly RabbitMqContainer _rabbitMq = new RabbitMqBuilder().Build();

    public string PostgresConnectionString => _postgres.GetConnectionString();
    public string RabbitMqConnectionString => _rabbitMq.GetConnectionString();

    public async Task InitializeAsync()
    {
        try
        {
            await _postgres.StartAsync();
            await _rabbitMq.StartAsync();
        }
        catch
        {
            await DisposeAsync();
            throw;
        }
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
        await _rabbitMq.DisposeAsync();
    }

    public IConfiguration BuildConfiguration(bool enableEventConsumers = false, Dictionary<string, string?>? overrides = null)
        => TestConfigurationFactory.Build(BuildConfigurationValues(enableEventConsumers, overrides));

    public Dictionary<string, string?> BuildConfigurationValues(bool enableEventConsumers = false, Dictionary<string, string?>? overrides = null)
    {
        var values = new Dictionary<string, string?>
        {
            ["ConnectionStrings:postgres"] = PostgresConnectionString,
            ["Messaging:EnableEventConsumers"] = enableEventConsumers.ToString().ToLowerInvariant()
        };

        if (enableEventConsumers)
        {
            ApplyRabbitMqSettings(values);
        }

        if (overrides is not null)
        {
            foreach (var pair in overrides)
            {
                values[pair.Key] = pair.Value;
            }
        }

        return values;
    }

    public ServiceProvider CreateServiceProvider(bool enableEventConsumers = false, Action<IServiceCollection>? configureServices = null)
    {
        var configuration = BuildConfiguration(enableEventConsumers);
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddGameEventConsumers(configuration);
        services.AddNotificationPersistence(configuration);
        services.AddSingleton<IGameNotificationPublisher, NoOpGameNotificationPublisher>();
        configureServices?.Invoke(services);

        var provider = services.BuildServiceProvider();
        provider.ApplyNotificationMigrations();
        return provider;
    }

    public async Task ResetDatabaseAsync(IServiceProvider provider)
    {
        await using var connection = new NpgsqlConnection(PostgresConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "DROP SCHEMA IF EXISTS \"public\" CASCADE; CREATE SCHEMA \"public\";";
        await command.ExecuteNonQueryAsync();

        provider.ApplyNotificationMigrations();
    }

    private void ApplyRabbitMqSettings(IDictionary<string, string?> values)
    {
        var rabbitUri = new Uri(RabbitMqConnectionString);
        var credentials = rabbitUri.UserInfo.Split(':', 2);
        var username = credentials.Length > 0 && !string.IsNullOrWhiteSpace(credentials[0]) ? credentials[0] : "guest";
        var password = credentials.Length > 1 && !string.IsNullOrWhiteSpace(credentials[1]) ? credentials[1] : "guest";

        values["Messaging:RabbitMq:Host"] = rabbitUri.Host;
        values["Messaging:RabbitMq:Port"] = rabbitUri.Port.ToString();
        values["Messaging:RabbitMq:VirtualHost"] = "/";
        values["Messaging:RabbitMq:Username"] = username;
        values["Messaging:RabbitMq:Password"] = password;
        values["ConnectionStrings:rabbitmq"] = RabbitMqConnectionString;
    }

    private sealed class NoOpGameNotificationPublisher : IGameNotificationPublisher
    {
        public Task PublishGameStateInitializedAsync(GameStateInitializedNotification notification, CancellationToken ct = default)
            => Task.CompletedTask;

        public Task PublishGameStateUpdatedAsync(GameStateUpdatedNotification notification, CancellationToken ct = default)
            => Task.CompletedTask;
    }
}
