using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace UserService.Configuration;

public static class UserCachingServiceCollectionExtensions
{
    public static IServiceCollection AddUserCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("redis");

        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "userservice";
            });

            return services;
        }

        services.AddDistributedMemoryCache();
        return services;
    }
}
