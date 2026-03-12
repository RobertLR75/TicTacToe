using FluentMigrator.Runner;
using GameNotificationService.Configuration;
using Microsoft.Extensions.Options;

namespace GameNotificationService.Persistence;

public static class NotificationPersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("postgres");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("ConnectionStrings:postgres is required for notification persistence.");
        }

        services.AddSingleton<IValidateOptions<NotificationQueryOptions>, NotificationQueryOptionsValidator>();
        services.AddOptions<NotificationQueryOptions>()
            .Bind(configuration.GetSection(NotificationQueryOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<NotificationPersistenceReadinessState>();
        services.AddSingleton<INotificationPersistenceInitializer, NotificationPersistenceInitializer>();
        services.AddHostedService<NotificationPersistenceInitializerHostedService>();
        services.AddSingleton<INotificationRepository>(_ => new PostgresNotificationRepository(connectionString));

        services.AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(NotificationPersistenceServiceCollectionExtensions).Assembly).For.Migrations());

        services.AddHealthChecks()
            .AddCheck<NotificationPersistenceHealthCheck>("notification_persistence");

        return services;
    }
}
