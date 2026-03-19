using GameNotificationService.Hubs;
using Microsoft.AspNetCore.SignalR;
using Service.Contracts.Notifications;
using SharedLibrary.SignalR;

namespace GameNotificationService.Services;

public interface ISignalRGameNotificationPublisher : ISignalRNotificationPublisher<GameHub>;
    
public class SignalRGameNotificationPublisher(
    IHubContext<GameHub> hubContext,
    ILogger<SignalRGameNotificationPublisher> logger) : SignalRNotificationBase<GameHub>(hubContext, logger), ISignalRGameNotificationPublisher
{
}
