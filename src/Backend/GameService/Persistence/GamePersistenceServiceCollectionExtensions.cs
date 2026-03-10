 using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;

namespace GameService.Persistence;

public static class GamePersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddGamePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("postgres");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("ConnectionStrings:postgres is required for game persistence.");
        }

        services.AddDbContext<GameDbContext>(options =>
            options.UseNpgsql(connectionString));
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<GameDbContext>());

        services.AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(GamePersistenceServiceCollectionExtensions).Assembly).For.Migrations());

        return services;
    }

    public static void ApplyGameMigrations(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}
