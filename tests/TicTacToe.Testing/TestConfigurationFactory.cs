using Microsoft.Extensions.Configuration;

namespace TicTacToe.Testing;

public static class TestConfigurationFactory
{
    public static IConfiguration Build(IEnumerable<KeyValuePair<string, string?>> values)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }
}

