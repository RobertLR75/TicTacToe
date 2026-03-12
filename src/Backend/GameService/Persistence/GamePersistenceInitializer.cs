using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GameService.Persistence;

public interface IGamePersistenceInitializer
{
    Task<bool> EnsureInitializedAsync(CancellationToken cancellationToken = default);
}

public sealed class GamePersistenceInitializer(
    IServiceScopeFactory scopeFactory,
    GamePersistenceReadinessState readinessState,
    ILogger<GamePersistenceInitializer> logger) : IGamePersistenceInitializer
{
    public async Task<bool> EnsureInitializedAsync(CancellationToken cancellationToken = default)
    {
        var attemptUtc = DateTimeOffset.UtcNow;

        try
        {
            using var scope = scopeFactory.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
            await Task.Run(runner.MigrateUp, cancellationToken);

            var becameReady = !readinessState.IsReady;
            readinessState.MarkReady(attemptUtc);

            if (becameReady)
            {
                logger.LogInformation("Game persistence initialization succeeded at {AttemptUtc}.", attemptUtc);
            }

            return true;
        }
        catch (Exception ex)
        {
            readinessState.MarkUnavailable(ex.Message, attemptUtc);
            logger.LogWarning(ex,
                "Game persistence initialization failed at {AttemptUtc}. The service will remain running and retry while PostgreSQL is unavailable.",
                attemptUtc);
            return false;
        }
    }
}

public static class GamePersistenceStartupExtensions
{
    private static readonly TimeSpan DefaultRetryDelay = TimeSpan.FromSeconds(5);

    public static Task EnsureGamePersistenceReadyBeforeStartupAsync(
        this IServiceProvider services,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        return WaitForGamePersistenceReadyAsync(services, logger, DefaultRetryDelay, cancellationToken);
    }

    public static async Task WaitForGamePersistenceReadyAsync(
        this IServiceProvider services,
        ILogger logger,
        TimeSpan retryDelay,
        CancellationToken cancellationToken = default)
    {
        var initializer = services.GetRequiredService<IGamePersistenceInitializer>();

        while (!cancellationToken.IsCancellationRequested)
        {
            if (await initializer.EnsureInitializedAsync(cancellationToken))
            {
                return;
            }

            logger.LogInformation("Retrying game persistence initialization in {RetryDelaySeconds} seconds.", retryDelay.TotalSeconds);
            await Task.Delay(retryDelay, cancellationToken);
        }
    }
}

public sealed class GamePersistenceHealthCheck(GamePersistenceReadinessState readinessState) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (readinessState.IsReady)
        {
            return Task.FromResult(HealthCheckResult.Healthy("Game persistence is ready."));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy(
            "Game persistence is unavailable.",
            data: new Dictionary<string, object>
            {
                ["lastAttemptUtc"] = readinessState.LastAttemptUtc?.ToString("O") ?? string.Empty,
                ["lastError"] = readinessState.LastErrorMessage ?? string.Empty
            }));
    }
}

