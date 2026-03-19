using GameService.Features.Games.Entities;
using GameService.Services;
using SharedLibrary.PostgreSql.EntityFramework;
using Xunit;

namespace GameService.UnitTests;

public sealed class GameStorageServiceUnitTests
{
    [Fact]
    public void Storage_service_inherits_base_postgres_storage_service_for_game_model()
    {
        Assert.Equal(typeof(EntityFrameworkPostgresSqlStorageBase<GameEntity>), typeof(GameStorageService).BaseType);
    }

    [Fact]
    public void Storage_service_implements_expected_storage_contract()
    {
        Assert.Contains(typeof(IGameStorageService), typeof(GameStorageService).GetInterfaces());
    }
}
