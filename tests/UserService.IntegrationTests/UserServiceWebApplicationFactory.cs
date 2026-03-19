using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace UserService.IntegrationTests;

public sealed class UserServiceWebApplicationFactory(string cosmosConnectionString, string rabbitMqConnectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var rabbitUri = new Uri(rabbitMqConnectionString);
        var credentials = rabbitUri.UserInfo.Split(':', 2);
        var username = credentials.Length > 0 && !string.IsNullOrWhiteSpace(credentials[0]) ? credentials[0] : "guest";
        var password = credentials.Length > 1 && !string.IsNullOrWhiteSpace(credentials[1]) ? credentials[1] : "guest";

        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:cosmos"] = cosmosConnectionString,
                ["ConnectionStrings:rabbitmq"] = rabbitMqConnectionString,
                ["Messaging:EnableEventPublishing"] = "true",
                ["Messaging:RabbitMq:Host"] = rabbitUri.Host,
                ["Messaging:RabbitMq:Port"] = rabbitUri.Port.ToString(),
                ["Messaging:RabbitMq:Username"] = username,
                ["Messaging:RabbitMq:Password"] = password,
                ["UserStorage:DatabaseName"] = $"users-{Guid.NewGuid():N}",
                ["UserStorage:ContainerName"] = "users"
            });
        });
    }
}
