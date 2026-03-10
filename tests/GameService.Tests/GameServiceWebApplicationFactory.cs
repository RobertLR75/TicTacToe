using GameService.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GameService.Tests;

public sealed class GameServiceWebApplicationFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__postgres", connectionString);
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:postgres"] = connectionString
            });
        });
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DbContext>();
        await db.Database.ExecuteSqlRawAsync("DROP TABLE IF EXISTS game;");
        await db.Database.ExecuteSqlRawAsync("DROP TABLE IF EXISTS player;");
        await db.Database.ExecuteSqlRawAsync("CREATE TABLE player (id varchar(36) PRIMARY KEY, name varchar(50) NOT NULL);");
        await db.Database.ExecuteSqlRawAsync("CREATE TABLE game (id uuid PRIMARY KEY, status varchar(20) NOT NULL, created_at_utc timestamptz NOT NULL, updated_at_utc timestamptz NULL, player1_id varchar(36) NOT NULL REFERENCES player(id), player2_id varchar(36) NULL REFERENCES player(id));");
        await db.Database.ExecuteSqlRawAsync("CREATE INDEX ix_game_status ON game(status);");
    }
}
