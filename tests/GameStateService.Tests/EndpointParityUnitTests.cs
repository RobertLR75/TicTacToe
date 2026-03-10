using FastEndpoints;
using GameStateService.Endpoints.Games.Create;
using GameStateService.Endpoints.Games.Get;
using GameStateService.Endpoints.Games.MakeMove;
using GameStateService.Services;
using GameStateService.Models;
using Xunit;

namespace GameStateService.Tests;

public class EndpointParityUnitTests
{
    [Fact]
    public void Create_endpoint_contract_remains_without_request_and_returns_create_response()
    {
        Assert.Equal(typeof(EndpointWithoutRequest<CreateGameResponse, CreateGameMapper>), typeof(CreateGameEndpoint).BaseType);
    }

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
    public void Endpoints_delegate_to_request_handlers()
    {
        var createCtor = typeof(CreateGameEndpoint).GetConstructors().Single();
        var getCtor = typeof(GetGameEndpoint).GetConstructors().Single();
        var moveCtor = typeof(MakeMoveEndpoint).GetConstructors().Single();

        Assert.Equal(typeof(IRequestHandler<CreateGameCommand, GameState>), createCtor.GetParameters().Single().ParameterType);
        Assert.Equal(typeof(IRequestHandler<GetGameQuery, GetGameQueryResult>), getCtor.GetParameters().Single().ParameterType);
        Assert.Equal(typeof(IRequestHandler<MakeMoveCommand, MakeMoveCommandResult>), moveCtor.GetParameters().Single().ParameterType);
    }

    [Fact]
    public void Create_response_shape_remains_gameId_only()
    {
        var properties = typeof(CreateGameResponse).GetProperties();
        Assert.Single(properties);
        Assert.Equal(nameof(CreateGameResponse.GameId), properties[0].Name);
    }
}
