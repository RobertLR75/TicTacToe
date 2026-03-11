using GameService.Persistence;
using GameService.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Service.Contracts.Shared;
using TicTacToe.Testing;

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
        return DatabaseSchemaReset.ResetAsync(Services);
    }

    private sealed class NoOpGameEventPublisher : IGameEventPublisher
    {
        public Task PublishEventAsync<T>(T @event, CancellationToken ct = default) where T : class, ISharedEvent
            => Task.CompletedTask;
    }
}
