namespace GameService.Persistence;

public static class PostgresConnectionStringResolver
{
    private static readonly string[] CandidateConnectionStringNames =
    [
        "postgres-db",
        "postgres"
    ];

    private static readonly string[] CandidateConfigurationKeys =
    [
        "ConnectionStrings:postgres-db",
        "ConnectionStrings:postgres",
        "Services:postgres-db:connectionString",
        "Services:postgres:connectionString"
    ];

    public static string ResolveRequired(IConfiguration configuration, string consumerName)
    {
        if (TryResolve(configuration, out var connectionString, out var resolvedFrom))
        {
            return connectionString;
        }

        throw new InvalidOperationException(
            $"PostgreSQL configuration is required for {consumerName}. Supported configuration paths: {string.Join(", ", GetSupportedConfigurationPaths())}." +
            " Ensure local configuration or Aspire resource binding provides one of these values.");
    }

    public static bool TryResolve(IConfiguration configuration, out string connectionString, out string? resolvedFrom)
    {
        foreach (var connectionName in CandidateConnectionStringNames)
        {
            var candidate = configuration.GetConnectionString(connectionName);
            if (!string.IsNullOrWhiteSpace(candidate))
            {
                connectionString = candidate;
                resolvedFrom = $"ConnectionStrings:{connectionName}";
                return true;
            }
        }

        foreach (var key in CandidateConfigurationKeys)
        {
            var candidate = configuration[key];
            if (!string.IsNullOrWhiteSpace(candidate))
            {
                connectionString = candidate;
                resolvedFrom = key;
                return true;
            }
        }

        connectionString = string.Empty;
        resolvedFrom = null;
        return false;
    }

    public static IReadOnlyList<string> GetSupportedConfigurationPaths()
    {
        return CandidateConfigurationKeys;
    }
}
