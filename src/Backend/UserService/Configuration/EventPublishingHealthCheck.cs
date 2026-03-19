using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace UserService.Configuration;

public sealed class EventPublishingHealthCheck(IOptions<MessagingOptions> options) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var config = options.Value;

        if (!config.EnableEventPublishing)
        {
            return Task.FromResult(HealthCheckResult.Healthy("Event publishing is disabled by configuration."));
        }

        if (string.IsNullOrWhiteSpace(config.RabbitMq.Host) ||
            config.RabbitMq.Port <= 0 ||
            string.IsNullOrWhiteSpace(config.RabbitMq.Username) ||
            string.IsNullOrWhiteSpace(config.RabbitMq.Password))
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Event publishing is enabled but RabbitMQ configuration is invalid."));
        }

        return Task.FromResult(HealthCheckResult.Healthy("Event publishing is enabled and RabbitMQ settings are configured."));
    }
}
