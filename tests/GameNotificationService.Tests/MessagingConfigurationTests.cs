using GameNotificationService.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TicTacToe.Testing;
using Xunit;

namespace GameNotificationService.Tests;

public class MessagingConfigurationTests
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

    private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
    {
        return TestConfigurationFactory.Build(values);
    }
}
