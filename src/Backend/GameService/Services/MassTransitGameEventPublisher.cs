using GameService.Configuration;
using MassTransit;
using Microsoft.Extensions.Options;
using Service.Contracts.Shared;

namespace GameService.Services;

public class MassTransitGameEventPublisher(
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
