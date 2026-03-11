using GameNotificationService.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace GameNotificationService.UnitTests;

public class MessagingConfigurationTests : GameNotificationServiceUnitTestBase
{
    [Fact]
    public void AddGameEventConsumers_throws_for_invalid_rabbitmq_configuration_when_enabled()
    {
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["Messaging:EnableEventConsumers"] = "true",
            ["Messaging:RabbitMq:Host"] = "",
            ["Messaging:RabbitMq:Port"] = "0",
            ["Messaging:RabbitMq:Username"] = "",
            ["Messaging:RabbitMq:Password"] = ""
        });

        var services = new ServiceCollection();
        services.AddGameEventConsumers(configuration);
        using var provider = services.BuildServiceProvider();

        Assert.Throws<OptionsValidationException>(() => _ = provider.GetRequiredService<IOptions<MessagingOptions>>().Value);
    }

    [Fact]
    public void AddGameEventConsumers_allows_missing_rabbitmq_configuration_when_disabled()
    {
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["Messaging:EnableEventConsumers"] = "false"
        });

        var services = new ServiceCollection();
        services.AddGameEventConsumers(configuration);
        using var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<MessagingOptions>>().Value;
        Assert.False(options.EnableEventConsumers);
    }

    [Fact]
    public void AddGameEventConsumers_uses_connection_string_fallback_when_rabbitmq_section_is_missing()
    {
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["Messaging:EnableEventConsumers"] = "true",
            ["ConnectionStrings:rabbitmq"] = "rabbitmq://user:pass@rabbit-host:5678/game-vhost"
        });

        var services = new ServiceCollection();
        services.AddGameEventConsumers(configuration);
        using var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<MessagingOptions>>().Value;
        Assert.True(options.EnableEventConsumers);
        Assert.Equal("rabbit-host", options.RabbitMq.Host);
        Assert.Equal(5678, options.RabbitMq.Port);
        Assert.Equal("game-vhost", options.RabbitMq.VirtualHost);
        Assert.Equal("user", options.RabbitMq.Username);
        Assert.Equal("pass", options.RabbitMq.Password);
    }
}
