using System.Diagnostics.CodeAnalysis;

namespace GameService.Persistence;

public sealed class GamePersistenceReadinessState
{
    private readonly object _sync = new();

    public bool IsReady { get; private set; }

    public string? LastErrorMessage { get; private set; }

    public DateTimeOffset? LastAttemptUtc { get; private set; }

    public DateTimeOffset? LastSuccessUtc { get; private set; }

    public void MarkReady(DateTimeOffset timestampUtc)
    {
        lock (_sync)
        {
            IsReady = true;
            LastAttemptUtc = timestampUtc;
            LastSuccessUtc = timestampUtc;
            LastErrorMessage = null;
        }
    }

    public void MarkUnavailable(string errorMessage, DateTimeOffset timestampUtc)
    {
        lock (_sync)
        {
            IsReady = false;
            LastAttemptUtc = timestampUtc;
            LastErrorMessage = errorMessage;
        }
    }

    public bool TryGetLastError([NotNullWhen(true)] out string? errorMessage)
    {
        lock (_sync)
        {
            errorMessage = LastErrorMessage;
            return !string.IsNullOrWhiteSpace(errorMessage);
        }
    }
}

