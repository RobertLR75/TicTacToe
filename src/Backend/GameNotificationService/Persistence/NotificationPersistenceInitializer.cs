using FluentMigrator.Runner;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GameNotificationService.Persistence;

public interface INotificationPersistenceInitializer
{
    Task<bool> EnsureInitializedAsync(CancellationToken cancellationToken = default);
}

public sealed class NotificationPersistenceInitializer(
    IServiceScopeFactory scopeFactory,
    NotificationPersistenceReadinessState readinessState,
    ILogger<NotificationPersistenceInitializer> logger) : INotificationPersistenceInitializer
{
    public async Task<bool> EnsureInitializedAsync(CancellationToken cancellationToken = default)
    {
        var attemptUtc = DateTimeOffset.UtcNow;

        try
        {
            using var scope = scopeFactory.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            await Task.Run(runner.MigrateUp, cancellationToken);

            var becameReady = !readinessState.IsReady;
            readinessState.MarkReady(attemptUtc);

            if (becameReady)
            {
                logger.LogInformation("Notification persistence initialization succeeded at {AttemptUtc}.", attemptUtc);
            }

            return true;
        }
        catch (Exception ex)
        {
            readinessState.MarkUnavailable(ex.Message, attemptUtc);
            logger.LogWarning(ex,
                "Notification persistence initialization failed at {AttemptUtc}. The service will remain running and retry while PostgreSQL is unavailable.",
                attemptUtc);
            return false;
        }
    }
}

public sealed class NotificationPersistenceInitializerHostedService(
    INotificationPersistenceInitializer initializer,
    ILogger<NotificationPersistenceInitializerHostedService> logger) : BackgroundService
{
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);
    private const int MaxConsecutiveRetriesBeforeLogDowngrade = 1000;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consecutiveFailures = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            var success = await initializer.EnsureInitializedAsync(stoppingToken);
            if (success)
            {
                return;
            }

            consecutiveFailures++;
            if (consecutiveFailures <= MaxConsecutiveRetriesBeforeLogDowngrade)
            {
                logger.LogInformation("Retrying notification persistence initialization in {RetryDelaySeconds} seconds.", RetryDelay.TotalSeconds);
            }

            await Task.Delay(RetryDelay, stoppingToken);
        }
    }
}

public sealed class NotificationPersistenceHealthCheck(NotificationPersistenceReadinessState readinessState) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (readinessState.IsReady)
        {
            return Task.FromResult(HealthCheckResult.Healthy("Notification persistence is ready."));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy(
            "Notification persistence is unavailable.",
            data: new Dictionary<string, object>
            {
                ["lastAttemptUtc"] = readinessState.LastAttemptUtc?.ToString("O") ?? string.Empty,
                ["lastError"] = readinessState.LastErrorMessage ?? string.Empty
            }));
    }
}
