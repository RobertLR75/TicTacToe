using Microsoft.Extensions.Options;

namespace GameNotificationService.Configuration;

public static class NotificationStorageServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IValidateOptions<NotificationQueryOptions>, NotificationQueryOptionsValidator>();
        services.AddOptions<NotificationQueryOptions>()
            .Bind(configuration.GetSection(NotificationQueryOptions.SectionName))
            .ValidateOnStart();

        return services;
    }
}
