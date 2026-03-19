using Testcontainers.CosmosDb;
using Xunit;

namespace UserService.IntegrationTests.Testing;

public sealed class CosmosDbFixture : IAsyncLifetime
{
    private readonly CosmosDbContainer _container = new CosmosDbBuilder()
        .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:vnext-preview")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public HttpClient HttpClient => _container.HttpClient;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}

[CollectionDefinition(Name)]
public sealed class CosmosDbCollection : ICollectionFixture<CosmosDbFixture>
{
    public const string Name = "cosmosdb-userservice";
}
