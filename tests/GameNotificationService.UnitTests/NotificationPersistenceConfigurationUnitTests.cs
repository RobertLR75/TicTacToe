using GameNotificationService.Configuration;
using GameNotificationService.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace GameNotificationService.UnitTests;

public sealed class NotificationPersistenceConfigurationUnitTests : GameNotificationServiceUnitTestBase
{
    [Fact]
    public void AddNotificationStorage_registers_query_options()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration([]);

        services.AddNotificationStorage(configuration);
        using var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<NotificationQueryOptions>>().Value;
        Assert.Equal(50, options.DefaultPageSize);
    }

    [Fact]
    public void AddNotificationStorage_and_redis_storage_register_expected_services()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["NotificationQuery:DefaultPageSize"] = "25",
            ["NotificationQuery:MaxPageSize"] = "100"
        });

        services.AddDistributedMemoryCache();
        services.AddNotificationStorage(configuration);
        services.AddScoped<INotificationStorageService, RedisNotificationStorageService>();
        using var provider = services.BuildServiceProvider();

        var repository = provider.GetRequiredService<INotificationStorageService>();
        var options = provider.GetRequiredService<IOptions<NotificationQueryOptions>>().Value;

        Assert.IsType<RedisNotificationStorageService>(repository);
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
