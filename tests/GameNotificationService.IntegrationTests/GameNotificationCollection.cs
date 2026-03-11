using Xunit;

namespace GameNotificationService.IntegrationTests;

[CollectionDefinition(Name)]
public sealed class GameNotificationCollection : ICollectionFixture<GameNotificationServiceIntegrationTestFixture>
{
    public const string Name = "gamenotificationservice-integration";
}

