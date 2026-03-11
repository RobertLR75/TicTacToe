using GameNotificationService.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Xunit;

namespace GameNotificationService.UnitTests;

public sealed class EventConsumerHealthCheckUnitTests
{
    [Fact]
    public async Task CheckHealthAsync_returns_healthy_when_consumers_are_disabled()
    {
        var sut = new EventConsumerHealthCheck(Options.Create(new MessagingOptions { EnableEventConsumers = false }));

        var result = await sut.CheckHealthAsync(new HealthCheckContext());

        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Contains("disabled", result.Description);
    }

    [Fact]
    public async Task CheckHealthAsync_returns_unhealthy_when_consumers_are_enabled_with_invalid_rabbitmq_settings()
    {
        var sut = new EventConsumerHealthCheck(Options.Create(new MessagingOptions
        {
            EnableEventConsumers = true,
            RabbitMq = new RabbitMqOptions
            {
                Host = "",
                Port = 0,
                Username = "",
                Password = ""
            }
        }));

        var result = await sut.CheckHealthAsync(new HealthCheckContext());

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
    }

    [Fact]
    public async Task CheckHealthAsync_returns_healthy_when_consumers_are_enabled_with_valid_rabbitmq_settings()
    {
        var sut = new EventConsumerHealthCheck(Options.Create(new MessagingOptions
        {
            EnableEventConsumers = true,
            RabbitMq = new RabbitMqOptions
            {
                Host = "localhost",
                Port = 5672,
                Username = "guest",
                Password = "guest"
            }
        }));

        var result = await sut.CheckHealthAsync(new HealthCheckContext());

        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Contains("configured", result.Description);
    }
}

