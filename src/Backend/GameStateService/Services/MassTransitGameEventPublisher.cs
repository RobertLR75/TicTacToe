using GameStateService.Configuration;
using MassTransit;
using Microsoft.Extensions.Options;

namespace GameStateService.Services;

public class MassTransitGameEventPublisher(
    IPublishEndpoint publishEndpoint,
    IOptions<MessagingOptions> messagingOptions) : IGameEventPublisher
{
    public async Task PublishEventAsync<T>(T @event, CancellationToken ct = default) where T : class
    {
        if (!messagingOptions.Value.EnableEventPublishing)
            return;

        await publishEndpoint.Publish(@event, ct);
    }
}
