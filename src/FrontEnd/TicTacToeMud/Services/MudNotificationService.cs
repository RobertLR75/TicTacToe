using MudBlazor;

namespace TicTacToeMud.Services;

public sealed class MudNotificationService(ISnackbar snackbar) : INotificationService
{
    public void ShowSuccess(string message)
    {
        snackbar.Add(message, Severity.Success);
    }

    public void ShowError(string message)
    {
        snackbar.Add(message, Severity.Error);
    }

    public void ShowWarning(string message)
    {
        snackbar.Add(message, Severity.Warning);
    }
}
