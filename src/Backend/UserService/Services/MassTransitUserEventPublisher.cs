using MassTransit;
using Microsoft.Extensions.Options;
using Service.Contracts.Shared;
using SharedLibrary.Interfaces;
using UserService.Configuration;

namespace UserService.Services;

public sealed class MassTransitUserEventPublisher(
    IPublishEndpoint publishEndpoint,
    IOptions<MessagingOptions> messagingOptions) : IUserEventPublisher
{
    public async Task PublishEventAsync<T>(T @event, CancellationToken ct = default) where T : class, ISharedEvent
    {
        if (!messagingOptions.Value.EnableEventPublishing)
            return;

        await publishEndpoint.Publish(@event, ct);
    }
}
