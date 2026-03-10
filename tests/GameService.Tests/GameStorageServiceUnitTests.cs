using GameService.Models;
using GameService.Services;
using SharedLibrary.PostgreSql.EntityFramework;
using Xunit;

namespace GameService.Tests;

public sealed class GameStorageServiceUnitTests
{
    [Fact]
    public void Storage_service_inherits_base_postgres_storage_service_for_game_model()
    {
        Assert.Equal(typeof(BasePostgresSqlStorageService<Game>), typeof(GameStorageService).BaseType);
    }

    [Fact]
    public void Storage_service_implements_expected_storage_contract()
    {
        Assert.Contains(typeof(IPostgresSqlStorageService<Game>), typeof(GameStorageService).GetInterfaces());
    }
}
