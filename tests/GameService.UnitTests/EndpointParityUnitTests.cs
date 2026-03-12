using FastEndpoints;
using GameService.Endpoints.Games.Create;
using GameService.Endpoints.Games.List;
using GameService.Endpoints.Games.UpdateStatus;
using GameService.Models;
using GameService.Services;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using Xunit;

namespace GameService.UnitTests;

public class EndpointParityUnitTests
{
    [Fact]
    public void Create_endpoint_contract_remains_request_response_pair()
    {
        Assert.Equal(typeof(Endpoint<CreateGameRequest, CreateGameResponse, CreateGameMapper>), typeof(CreateGameEndpoint).BaseType);
    }

    [Fact]
    public void List_endpoint_contract_remains_request_response_pair()
    {
        Assert.Equal(typeof(Endpoint<ListGamesRequest, ListGamesResponse, ListGamesMapper>), typeof(ListGamesEndpoint).BaseType);
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

        Assert.Equal(
            new[] { typeof(IRequestHandler<CreateGameCommand, Game>) },
            createCtor.GetParameters().Select(parameter => parameter.ParameterType).ToArray());
        Assert.Equal(
            new[] { typeof(IRequestHandler<ListGamesQuery, IEnumerable<Game>>) },
            listCtor.GetParameters().Select(parameter => parameter.ParameterType).ToArray());
        Assert.Equal(
            new[] { typeof(IRequestHandler<UpdateGameStatusCommand, GameStatusUpdateResult>) },
            updateCtor.GetParameters().Select(parameter => parameter.ParameterType).ToArray());
    }
}
