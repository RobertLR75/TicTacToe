using Xunit;

namespace UserService.IntegrationTests.Testing;

[CollectionDefinition(Name)]
public sealed class UserServiceCollection : ICollectionFixture<CosmosDbFixture>, ICollectionFixture<RabbitMqFixture>
{
    public const string Name = "userservice-integration";
}
