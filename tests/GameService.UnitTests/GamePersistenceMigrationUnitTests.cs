using FluentMigrator;
using GameService.Persistence.Migrations;
using SharedLibrary.FluentMigration;
using Xunit;
using DomainGame = GameService.Models.Game;

namespace GameService.UnitTests;

public sealed class GamePersistenceMigrationUnitTests
{
    [Fact]
    public void CreateGameAndPlayerTables_keeps_expected_migration_contract()
    {
        var migrationType = typeof(CreateGameAndPlayerTables);
        var migrationAttribute = Assert.Single(migrationType.GetCustomAttributes(typeof(MigrationAttribute), inherit: false));

        Assert.Equal(2026030601, ((MigrationAttribute)migrationAttribute).Version);
        Assert.True(typeof(GenericTableMigration<DomainGame>).IsAssignableFrom(migrationType));
        Assert.Equal(migrationType, migrationType.GetMethod(nameof(CreateGameAndPlayerTables.Up))!.DeclaringType);
    }
}

