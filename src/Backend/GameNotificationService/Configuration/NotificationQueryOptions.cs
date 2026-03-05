using Microsoft.Extensions.Options;

namespace GameNotificationService.Configuration;

public sealed class NotificationQueryOptions
{
    public const string SectionName = "NotificationQuery";

    public int DefaultPageSize { get; set; } = 50;

    public int MaxPageSize { get; set; } = 200;
}

public sealed class NotificationQueryOptionsValidator : IValidateOptions<NotificationQueryOptions>
{
    public ValidateOptionsResult Validate(string? name, NotificationQueryOptions options)
    {
        var failures = new List<string>();

        if (options.DefaultPageSize <= 0)
            failures.Add("NotificationQuery:DefaultPageSize must be greater than 0.");

        if (options.MaxPageSize <= 0)
            failures.Add("NotificationQuery:MaxPageSize must be greater than 0.");

        if (options.DefaultPageSize > options.MaxPageSize)
            failures.Add("NotificationQuery:DefaultPageSize must be less than or equal to NotificationQuery:MaxPageSize.");

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
