using GameStateService.Configuration;
using MassTransit;
using Microsoft.Extensions.Options;
using Service.Contracts.Shared;

namespace GameStateService.Services;

public class MassTransitGameStateEventPublisher(
    IPublishEndpoint publishEndpoint,
    IOptions<MessagingOptions> messagingOptions) : IGameEventPublisher
{
    public async Task PublishEventAsync<T>(T @event, CancellationToken ct = default) where T : class, ISharedEvent
    {
        if (!messagingOptions.Value.EnableEventPublishing)
            return;

        await publishEndpoint.Publish(@event, ct);
    }
}
