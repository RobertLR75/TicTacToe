namespace TicTacToe.Testing;

public static class AsyncPolling
{
    public static async Task<bool> WaitForAsync(
        Func<Task<bool>> condition,
        TimeSpan timeout,
        TimeSpan? pollInterval = null,
        CancellationToken cancellationToken = default)
    {
        var interval = pollInterval ?? TimeSpan.FromMilliseconds(250);
        var started = DateTimeOffset.UtcNow;

        while (DateTimeOffset.UtcNow - started < timeout)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (await condition())
            {
                return true;
            }

            await Task.Delay(interval, cancellationToken);
        }

        return false;
    }
}

