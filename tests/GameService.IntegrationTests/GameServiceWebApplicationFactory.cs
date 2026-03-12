using GameService.Persistence;
using GameService.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Service.Contracts.Shared;

namespace GameService.IntegrationTests;

public sealed class GameServiceWebApplicationFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__postgres", connectionString);
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:postgres"] = connectionString
            });
        });
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IGameEventPublisher>();
            services.AddScoped<IGameEventPublisher, NoOpGameEventPublisher>();
        });
    }

    public Task ResetDatabaseAsync()
    {
        return ResetDatabaseAndWaitForPersistenceReadyAsync();
    }

    private async Task ResetDatabaseAndWaitForPersistenceReadyAsync()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "DROP SCHEMA IF EXISTS \"public\" CASCADE; CREATE SCHEMA \"public\";";
        await command.ExecuteNonQueryAsync();

        await WaitForPersistenceReadyAsync(Services);
    }

    private static async Task WaitForPersistenceReadyAsync(IServiceProvider services)
    {
        var initializer = services.GetRequiredService<IGamePersistenceInitializer>();
        var readinessState = services.GetRequiredService<GamePersistenceReadinessState>();

        for (var attempt = 0; attempt < 10; attempt++)
        {
            if (await initializer.EnsureInitializedAsync())
            {
                return;
            }

            await Task.Delay(200);
        }

        throw new InvalidOperationException(
            $"Game persistence initializer could not prepare the test database. Last error: {readinessState.LastErrorMessage ?? "unknown"}");
    }

    private sealed class NoOpGameEventPublisher : IGameEventPublisher
    {
        public Task PublishEventAsync<T>(T @event, CancellationToken ct = default) where T : class, ISharedEvent
            => Task.CompletedTask;
    }
}
