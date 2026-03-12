
using FastEndpoints;

public class CreateOrderEndpoint : Endpoint<CreateOrderRequest, CreateOrderResponse>
{
    public override void Configure()
    {
        Post("/orders");
    }

    public override async Task HandleAsync(CreateOrderRequest req, CancellationToken ct)
    {
        var response = new CreateOrderResponse
        {
            OrderId = Guid.NewGuid()
        };

        await SendAsync(response);
    }
}
