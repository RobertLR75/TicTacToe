using MassTransit;
using MassTransit.RabbitMqTransport;

namespace GameStateService.Tests.Testing;

public abstract class IntegrationTestBase
{
    protected static async Task<bool> WaitForAsync(Func<Task<bool>> condition, TimeSpan timeout)
    {
        var started = DateTimeOffset.UtcNow;
        while (DateTimeOffset.UtcNow - started < timeout)
        {
            if (await condition())
            {
                return true;
            }

            await Task.Delay(200);
        }

        return false;
    }

    protected static IBusControl BuildRabbitMqListener(
        string queueName,
        Uri rabbitMqUri,
        string username,
        string password,
        Action<IRabbitMqReceiveEndpointConfigurator> configureHandler)
    {
        return Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host(rabbitMqUri, h =>
            {
                h.Username(username);
                h.Password(password);
            });

            cfg.ReceiveEndpoint(queueName, configureHandler);
        });
    }
}
