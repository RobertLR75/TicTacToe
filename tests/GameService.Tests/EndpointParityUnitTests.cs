using FastEndpoints;
using GameService.Contracts;
using GameService.Endpoints.Games.Create;
using GameService.Endpoints.Games.List;
using GameService.Endpoints.Games.UpdateStatus;
using GameService.Models;
using GameService.Services;
using SharedLibrary.PostgreSql.EntityFramework;
using Xunit;

namespace GameService.Tests;

public class EndpointParityUnitTests
{
    [Fact]
    public void Create_endpoint_contract_remains_request_response_pair()
    {
        Assert.Equal(typeof(Endpoint<CreateGameRequest, CreateGameResponse>), typeof(CreateGameEndpoint).BaseType);
    }

    [Fact]
    public void List_endpoint_contract_remains_request_response_pair()
    {
        Assert.Equal(typeof(Endpoint<ListGamesRequest, ListGamesResponse>), typeof(ListGamesEndpoint).BaseType);
    }

    [Fact]
    public void Update_status_endpoint_contract_uses_mapper_based_endpoint()
    {
        Assert.Equal(
            typeof(Endpoint<UpdateGameStatusRequest, UpdateGameStatusResponse, UpdateGameStatusMapper>),
            typeof(UpdateGameStatusEndpoint).BaseType);
    }

    [Fact]
    public void Endpoints_delegate_to_expected_dependencies()
    {
        var createCtor = typeof(CreateGameEndpoint).GetConstructors().Single();
        var listCtor = typeof(ListGamesEndpoint).GetConstructors().Single();
        var updateCtor = typeof(UpdateGameStatusEndpoint).GetConstructors().Single();

        Assert.Equal(typeof(IPostgresSqlStorageService<GameModel>), createCtor.GetParameters().Single().ParameterType);
        Assert.Equal(typeof(IPostgresSqlStorageService<GameModel>), listCtor.GetParameters().Single().ParameterType);
        Assert.Equal(typeof(IRequestHandler<UpdateGameStatusCommand, GameStatusUpdateResult>), updateCtor.GetParameters().Single().ParameterType);
    }
}
