using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Xunit;
using GameService.Persistence;

namespace GameService.IntegrationTests;

[Collection(PostgresCollection.Name)]
public sealed class GamePersistenceMigrationIntegrationTests : GameServiceIntegrationTestBase
{
    public GamePersistenceMigrationIntegrationTests(PostgresTestContainerFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task EnsureInitializedAsync_applies_expected_game_and_player_tables()
    {
        await using var provider = CreateServiceProvider();
        await ResetDatabaseAsync(provider);

        using var scope = provider.CreateScope();
        var readinessState = scope.ServiceProvider.GetRequiredService<GamePersistenceReadinessState>();

        Assert.True(readinessState.IsReady);

        await using var connection = new NpgsqlConnection(Fixture.ConnectionString);
        await connection.OpenAsync();

        var publicTables = await GetPublicTablesAsync(connection);
        var playerTable = FindTable(publicTables, "player");
        var gameTable = FindTable(publicTables, "game");
        var playerColumns = publicTables[playerTable];
        var gameColumns = publicTables[gameTable];

        Assert.NotEqual(playerTable, gameTable);

        Assert.Equal(["id", "name"], playerColumns.Keys.Order().ToArray());
        Assert.Equal(["created_at_utc", "id", "player1_id", "player2_id", "status", "updated_at_utc"], gameColumns.Keys.Order().ToArray());

        Assert.Equal("NO", playerColumns["id"]);
        Assert.Equal("NO", playerColumns["name"]);

        Assert.Equal("NO", gameColumns["id"]);
        Assert.Equal("NO", gameColumns["status"]);
        Assert.Equal("NO", gameColumns["created_at_utc"]);
        Assert.Equal("YES", gameColumns["updated_at_utc"]);
        Assert.Equal("NO", gameColumns["player1_id"]);
        Assert.Equal("YES", gameColumns["player2_id"]);
    }

    [Fact]
    public async Task EnsureInitializedAsync_creates_player_foreign_keys_and_status_index()
    {
        await using var provider = CreateServiceProvider();
        await ResetDatabaseAsync(provider);

        await using var connection = new NpgsqlConnection(Fixture.ConnectionString);
        await connection.OpenAsync();

        var publicTables = await GetPublicTablesAsync(connection);
        var playerTable = FindTable(publicTables, "player");
        var gameTable = FindTable(publicTables, "game");
        var foreignKeys = await GetForeignKeysAsync(connection, gameTable);
        var indexDefinitions = await GetIndexDefinitionsAsync(connection, gameTable);

        Assert.Contains(foreignKeys, fk => fk.ColumnName == "player1_id" && fk.ReferencedTable == playerTable && fk.ReferencedColumn == "id");
        Assert.Contains(foreignKeys, fk => fk.ColumnName == "player2_id" && fk.ReferencedTable == playerTable && fk.ReferencedColumn == "id");
        Assert.Contains(indexDefinitions, definition => definition.Contains("status", StringComparison.OrdinalIgnoreCase));
    }

    private static async Task<Dictionary<string, Dictionary<string, string>>> GetPublicTablesAsync(NpgsqlConnection connection)
    {
        const string sql = """
            SELECT table_name, column_name, is_nullable
            FROM information_schema.columns
            WHERE table_schema = 'public'
            ORDER BY table_name, column_name;
            """;

        await using var command = new NpgsqlCommand(sql, connection);

        var tables = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var tableName = reader.GetString(0);
            var columnName = reader.GetString(1);
            var isNullable = reader.GetString(2);

            if (!tables.TryGetValue(tableName, out var columns))
            {
                columns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                tables[tableName] = columns;
            }

            columns[columnName] = isNullable;
        }

        return tables;
    }

    private static async Task<List<ForeignKeyDefinition>> GetForeignKeysAsync(NpgsqlConnection connection, string tableName)
    {
        const string sql = """
            SELECT kcu.column_name, ccu.table_name, ccu.column_name
            FROM information_schema.table_constraints AS tc
            INNER JOIN information_schema.key_column_usage AS kcu
                ON tc.constraint_name = kcu.constraint_name
               AND tc.table_schema = kcu.table_schema
            INNER JOIN information_schema.constraint_column_usage AS ccu
                ON tc.constraint_name = ccu.constraint_name
               AND tc.table_schema = ccu.table_schema
            WHERE tc.table_schema = 'public'
              AND tc.table_name = @tableName
              AND tc.constraint_type = 'FOREIGN KEY';
            """;

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("tableName", tableName);
        await using var reader = await command.ExecuteReaderAsync();

        var foreignKeys = new List<ForeignKeyDefinition>();
        while (await reader.ReadAsync())
        {
            foreignKeys.Add(new ForeignKeyDefinition(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2)));
        }

        return foreignKeys;
    }

    private static async Task<List<string>> GetIndexDefinitionsAsync(NpgsqlConnection connection, string tableName)
    {
        var sql = string.Join(Environment.NewLine,
            "SELECT indexdef",
            "FROM pg_indexes",
            "WHERE schemaname = 'public' AND tablename = @tableName;");

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("tableName", tableName);

        var definitions = new List<string>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            definitions.Add(reader.GetString(0));
        }

        return definitions;
    }

    private static string FindTable(IReadOnlyDictionary<string, Dictionary<string, string>> publicTables, string tableName)
    {
        Assert.True(
            publicTables.ContainsKey(tableName),
            $"Expected migrated table '{tableName}' to exist. Public tables: {DescribeTables(publicTables)}");

        return tableName;
    }

    private static string DescribeTables(IReadOnlyDictionary<string, Dictionary<string, string>> publicTables)
    {
        if (publicTables.Count == 0)
        {
            return "<none>";
        }

        return string.Join(
            "; ",
            publicTables.OrderBy(table => table.Key)
                .Select(table => $"{table.Key}=[{string.Join(", ", table.Value.OrderBy(column => column.Key).Select(column => $"{column.Key}:{column.Value}"))}]"));
    }

    private sealed record ForeignKeyDefinition(string ColumnName, string ReferencedTable, string ReferencedColumn);
}






