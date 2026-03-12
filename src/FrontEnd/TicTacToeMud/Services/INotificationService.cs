namespace TicTacToeMud.Services;

public interface INotificationService
{
    void ShowSuccess(string message);
    void ShowError(string message);
    void ShowWarning(string message);
}
