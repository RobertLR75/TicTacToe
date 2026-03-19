using MassTransit;
using Microsoft.Extensions.Options;

namespace UserService.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserEventPublishing(this IServiceCollection services, IConfiguration configuration)
    {
        var messagingOptions = configuration.GetSection(MessagingOptions.SectionName).Get<MessagingOptions>() ?? new MessagingOptions();
        var rabbitMqConnectionString =
            configuration.GetConnectionString("rabbitmq") ??
            configuration.GetConnectionString("messaging");
        ApplyRabbitMqConnectionStringFallback(messagingOptions, rabbitMqConnectionString);

        services.AddSingleton<IValidateOptions<MessagingOptions>, MessagingOptionsValidator>();

        services.AddOptions<MessagingOptions>()
            .Bind(configuration.GetSection(MessagingOptions.SectionName))
            .PostConfigure(options => ApplyRabbitMqConnectionStringFallback(options, rabbitMqConnectionString))
            .ValidateOnStart();

        services.AddMassTransit(x =>
        {
            if (messagingOptions.EnableEventPublishing)
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    var virtualHost = messagingOptions.RabbitMq.VirtualHost?.Trim('/') ?? string.Empty;
                    var uri = string.IsNullOrWhiteSpace(virtualHost)
                        ? $"rabbitmq://{messagingOptions.RabbitMq.Host}:{messagingOptions.RabbitMq.Port}/"
                        : $"rabbitmq://{messagingOptions.RabbitMq.Host}:{messagingOptions.RabbitMq.Port}/{virtualHost}";

                    cfg.Host(new Uri(uri), h =>
                    {
                        h.Username(messagingOptions.RabbitMq.Username);
                        h.Password(messagingOptions.RabbitMq.Password);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            }
            else
            {
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            }
        });

        services.AddHealthChecks()
            .AddCheck<EventPublishingHealthCheck>("event_publishing_configuration");

        return services;
    }

    private static void ApplyRabbitMqConnectionStringFallback(MessagingOptions options, string? connectionString)
    {
        if (!options.EnableEventPublishing ||
            string.IsNullOrWhiteSpace(connectionString) ||
            !Uri.TryCreate(connectionString, UriKind.Absolute, out var rabbitMqUri))
        {
            return;
        }

        options.RabbitMq.Host = rabbitMqUri.Host;
        options.RabbitMq.Port = rabbitMqUri.Port;

        if (!string.IsNullOrWhiteSpace(rabbitMqUri.AbsolutePath) && rabbitMqUri.AbsolutePath != "/")
        {
            options.RabbitMq.VirtualHost = Uri.UnescapeDataString(rabbitMqUri.AbsolutePath.TrimStart('/'));
        }

        if (string.IsNullOrWhiteSpace(rabbitMqUri.UserInfo))
        {
            return;
        }

        var credentials = rabbitMqUri.UserInfo.Split(':', 2);
        if (credentials.Length == 2)
        {
            options.RabbitMq.Username = Uri.UnescapeDataString(credentials[0]);
            options.RabbitMq.Password = Uri.UnescapeDataString(credentials[1]);
        }
    }
}
