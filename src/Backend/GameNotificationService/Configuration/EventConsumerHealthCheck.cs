using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace GameNotificationService.Configuration;

public sealed class EventConsumerHealthCheck(IOptions<MessagingOptions> options) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var config = options.Value;

        if (!config.EnableEventConsumers)
        {
            return Task.FromResult(HealthCheckResult.Healthy("Event consumers are disabled by configuration."));
        }

        if (string.IsNullOrWhiteSpace(config.RabbitMq.Host) ||
            config.RabbitMq.Port <= 0 ||
            string.IsNullOrWhiteSpace(config.RabbitMq.Username) ||
            string.IsNullOrWhiteSpace(config.RabbitMq.Password))
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Event consumers are enabled but RabbitMQ configuration is invalid."));
        }

        return Task.FromResult(HealthCheckResult.Healthy("Event consumers are enabled and RabbitMQ settings are configured."));
    }
}
