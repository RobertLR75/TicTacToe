using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TicTacToe.Testing;

public static class DatabaseSchemaReset
{
    public static async Task ResetAsync(IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DbContext>();

        var resetStatements = new[]
        {
            "DROP SCHEMA IF EXISTS \"public\" CASCADE",
            "CREATE SCHEMA \"public\""
        };

        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = string.Join("; ", resetStatements) + ";";

        if (command.Connection?.State != System.Data.ConnectionState.Open)
        {
            await command.Connection!.OpenAsync();
        }

        await command.ExecuteNonQueryAsync();
        await db.Database.EnsureCreatedAsync();
    }
}

