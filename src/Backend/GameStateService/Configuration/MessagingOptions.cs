using Microsoft.Extensions.Options;

namespace GameStateService.Configuration;

public sealed class MessagingOptions
{
    public const string SectionName = "Messaging";

    public bool EnableEventPublishing { get; set; }

    public RabbitMqOptions RabbitMq { get; set; } = new();
}

public sealed class RabbitMqOptions
{
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; } = 5672;

    public string VirtualHost { get; set; } = "/";

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

public sealed class MessagingOptionsValidator : IValidateOptions<MessagingOptions>
{
    public ValidateOptionsResult Validate(string? name, MessagingOptions options)
    {
        if (!options.EnableEventPublishing)
        {
            return ValidateOptionsResult.Success;
        }

        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.RabbitMq.Host))
            failures.Add("Messaging:RabbitMq:Host is required when Messaging:EnableEventPublishing is true.");

        if (options.RabbitMq.Port <= 0)
            failures.Add("Messaging:RabbitMq:Port must be greater than 0 when Messaging:EnableEventPublishing is true.");

        if (string.IsNullOrWhiteSpace(options.RabbitMq.Username))
            failures.Add("Messaging:RabbitMq:Username is required when Messaging:EnableEventPublishing is true.");

        if (string.IsNullOrWhiteSpace(options.RabbitMq.Password))
            failures.Add("Messaging:RabbitMq:Password is required when Messaging:EnableEventPublishing is true.");

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
