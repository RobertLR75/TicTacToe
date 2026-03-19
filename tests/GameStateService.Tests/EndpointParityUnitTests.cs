using GameStateService.Features.GameStates.Endpoints.Get;
using GameStateService.Features.GameStates.Endpoints.Update;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using SharedLibrary.FastEndpoints;
using Xunit;

namespace GameStateService.Tests;

public class EndpointParityUnitTests
{
    [Fact]
    public void Get_endpoint_contract_remains_request_response_pair()
    {
        Assert.Equal(
            typeof(BaseQueryEndpoint<GetGameRequest, GetGameStateResponse, GetGameQuery, GameStateService.Features.GameStates.Entities.GameEntity, GetGameStateMapper>),
            typeof(GetGameStateEndpoint).BaseType);
    }

    [Fact]
    public void MakeMove_endpoint_contract_remains_request_only()
    {
        Assert.Equal(
            typeof(BaseCommandEndpoint<UpdateGameStateRequest, UpdateGameStateResponse, UpdateGameStateCommand, MakeMoveCommandResult, UpdateGameStateMapper>),
            typeof(UpdateGameStateEndpoint).BaseType);
    }

    [Fact]
    public void Remaining_endpoints_delegate_to_request_handlers()
    {
        var getCtor = typeof(GetGameStateEndpoint).GetConstructors().Single();
        var moveCtor = typeof(UpdateGameStateEndpoint).GetConstructors().Single();

        Assert.Equal(typeof(IGetGameHandler), getCtor.GetParameters().Single().ParameterType);
        Assert.Equal(typeof(IUpdateGameStateHandler), moveCtor.GetParameters().Single().ParameterType);
    }
}
