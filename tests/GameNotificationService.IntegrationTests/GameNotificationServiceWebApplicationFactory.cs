using GameNotificationService.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace GameNotificationService.IntegrationTests;

public sealed class GameNotificationServiceWebApplicationFactory(Dictionary<string, string?> configurationValues) : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        if (configurationValues.TryGetValue("ConnectionStrings:postgres", out var postgresConnectionString))
        {
            Environment.SetEnvironmentVariable("ConnectionStrings__postgres", postgresConnectionString);
        }

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(configurationValues);
        });
    }

    public async Task ResetDatabaseAsync()
    {
        if (!configurationValues.TryGetValue("ConnectionStrings:postgres", out var connectionString) || string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("ConnectionStrings:postgres is required to reset the notification test database.");
        }

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "DROP SCHEMA IF EXISTS \"public\" CASCADE; CREATE SCHEMA \"public\";";
        await command.ExecuteNonQueryAsync();

        await WaitForPersistenceReadyAsync(Services);
    }

    private static async Task WaitForPersistenceReadyAsync(IServiceProvider services)
    {
        var initializer = services.GetRequiredService<INotificationPersistenceInitializer>();
        var readinessState = services.GetRequiredService<NotificationPersistenceReadinessState>();

        for (var attempt = 0; attempt < 10; attempt++)
        {
            if (await initializer.EnsureInitializedAsync())
            {
                return;
            }

            await Task.Delay(200);
        }

        throw new InvalidOperationException(
            $"Notification persistence initializer could not prepare the test database. Last error: {readinessState.LastErrorMessage ?? "unknown"}");
    }
}
