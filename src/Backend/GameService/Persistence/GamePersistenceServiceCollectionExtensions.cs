using FluentMigrator.Runner;
using GameService.Models;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.PostgreSql.EntityFramework;

namespace GameService.Persistence;

public static class GamePersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddGamePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = PostgresConnectionStringResolver.ResolveRequired(configuration, "game persistence");

        services.AddSingleton<GamePersistenceReadinessState>();
        services.AddSingleton<IGamePersistenceInitializer, GamePersistenceInitializer>();

        services.AddDbContext<GenericDbContext<Game>>(options =>
            options.UseNpgsql(connectionString));
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<GenericDbContext<Game>>());

        services.AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(GamePersistenceServiceCollectionExtensions).Assembly).For.Migrations());

        services.AddHealthChecks()
            .AddCheck<GamePersistenceHealthCheck>("game_persistence");

        return services;
    }

    public static void ApplyGameMigrations(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}
