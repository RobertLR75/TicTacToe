using Testcontainers.RabbitMq;
using Xunit;

namespace GameStateService.Tests.Testing;

public sealed class RabbitMqFixture : IAsyncLifetime
{
    private readonly RabbitMqContainer _rabbitMq = new RabbitMqBuilder().Build();

    public string ConnectionString => _rabbitMq.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _rabbitMq.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _rabbitMq.DisposeAsync();
    }
}

[CollectionDefinition(Name)]
public sealed class RabbitMqCollection : ICollectionFixture<RabbitMqFixture>
{
    public const string Name = "rabbitmq-gamestate-service";
}
