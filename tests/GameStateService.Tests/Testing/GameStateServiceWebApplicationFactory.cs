using GameStateService.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GameStateService.Tests.Testing;

public sealed class GameStateServiceWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _rabbitMqConnectionString;
    private readonly IGameRepository? _repositoryOverride;
    private readonly bool _enableEventPublishing;

    public GameStateServiceWebApplicationFactory(
        string rabbitMqConnectionString,
        IGameRepository? repositoryOverride = null,
        bool enableEventPublishing = true)
    {
        _rabbitMqConnectionString = rabbitMqConnectionString;
        _repositoryOverride = repositoryOverride;
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

        if (_repositoryOverride is null)
            return;

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IGameRepository>();
            services.AddSingleton(_repositoryOverride);
        });
    }
}
