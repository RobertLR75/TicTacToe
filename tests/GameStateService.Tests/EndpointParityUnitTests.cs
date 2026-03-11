using FastEndpoints;
using GameStateService.Endpoints.Games.Get;
using GameStateService.Endpoints.Games.MakeMove;
using GameStateService.GameState;
using GameStateService.Models;
using GameStateService.Services;
using Service.Contracts.Responses;
using Xunit;

namespace GameStateService.Tests;

public class EndpointParityUnitTests
{
    [Fact]
    public void Get_endpoint_contract_remains_request_response_pair()
    {
        Assert.Equal(typeof(Endpoint<GetGameRequest, GetGameResponse>), typeof(GetGameEndpoint).BaseType);
    }

    [Fact]
    public void MakeMove_endpoint_contract_remains_request_only()
    {
        Assert.Equal(typeof(Endpoint<MakeMoveRequest>), typeof(MakeMoveEndpoint).BaseType);
    }

    [Fact]
    public void Remaining_endpoints_delegate_to_request_handlers()
    {
        var getCtor = typeof(GetGameEndpoint).GetConstructors().Single();
        var moveCtor = typeof(MakeMoveEndpoint).GetConstructors().Single();

        Assert.Equal(typeof(IRequestHandler<GetGame, GetGameQueryResult>), getCtor.GetParameters().Single().ParameterType);
        Assert.Equal(typeof(IRequestHandler<MakeMove, MakeMoveCommandResult>), moveCtor.GetParameters().Single().ParameterType);
    }
}
