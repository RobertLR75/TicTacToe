using GameNotificationService.Configuration;
using GameNotificationService.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace GameNotificationService.UnitTests;

public sealed class NotificationPersistenceConfigurationUnitTests : GameNotificationServiceUnitTestBase
{
    [Fact]
    public void AddNotificationPersistence_throws_when_postgres_connection_string_is_missing()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration([]);

        var ex = Assert.Throws<InvalidOperationException>(() => services.AddNotificationPersistence(configuration));

        Assert.Contains("ConnectionStrings:postgres", ex.Message);
    }

    [Fact]
    public void AddNotificationPersistence_registers_repository_and_query_options()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["ConnectionStrings:postgres"] = "Host=localhost;Database=test;Username=test;Password=test",
            ["NotificationQuery:DefaultPageSize"] = "25",
            ["NotificationQuery:MaxPageSize"] = "100"
        });

        services.AddNotificationPersistence(configuration);
        using var provider = services.BuildServiceProvider();

        var repository = provider.GetRequiredService<INotificationRepository>();
        var options = provider.GetRequiredService<IOptions<NotificationQueryOptions>>().Value;

        Assert.IsType<PostgresNotificationRepository>(repository);
        Assert.Equal(25, options.DefaultPageSize);
        Assert.Equal(100, options.MaxPageSize);
    }

    [Fact]
    public void NotificationQueryOptionsValidator_rejects_invalid_sizes()
    {
        var sut = new NotificationQueryOptionsValidator();

        var result = sut.Validate(null, new NotificationQueryOptions
        {
            DefaultPageSize = 10,
            MaxPageSize = 5
        });

        Assert.True(result.Failed);
        Assert.Contains(result.Failures, message => message.Contains("DefaultPageSize must be less than or equal"));
    }

    [Fact]
    public void NotificationQueryOptionsValidator_accepts_valid_sizes()
    {
        var sut = new NotificationQueryOptionsValidator();

        var result = sut.Validate(null, new NotificationQueryOptions
        {
            DefaultPageSize = 25,
            MaxPageSize = 100
        });

        Assert.True(result.Succeeded);
    }
}

