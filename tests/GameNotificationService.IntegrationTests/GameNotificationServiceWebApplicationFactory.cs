using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace GameNotificationService.IntegrationTests;

public sealed class GameNotificationServiceWebApplicationFactory(Dictionary<string, string?> configurationValues) : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        if (configurationValues.TryGetValue("ConnectionStrings:redis", out var redisConnectionString))
        {
            Environment.SetEnvironmentVariable("ConnectionStrings__redis", redisConnectionString);
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
}
