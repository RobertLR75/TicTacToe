using FastEndpoints;
using GameService.Features.Games.Endpoints.Create;
using GameService.Features.Games.Endpoints.Get;
using GameService.Features.Games.Endpoints.List;
using GameService.Features.Games.Endpoints.UpdateStatus;
using GameService.Features.Games.Entities;
using GameService.Services;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using SharedLibrary.FastEndpoints;
using Xunit;

namespace GameService.UnitTests;

public class EndpointParityUnitTests
{
    [Fact]
    public void Create_endpoint_contract_remains_request_response_pair()
    {
        Assert.Equal(
            typeof(BaseCommandEndpoint<CreateGameRequest, CreateGameResponse, CreateGameCommand, GameEntity, CreateGameMapper>),
            typeof(CreateGameEndpoint).BaseType);
    }

    [Fact]
    public void List_endpoint_contract_remains_request_response_pair()
    {
        Assert.Equal(
            typeof(BaseQueryEndpoint<ListGamesRequest, ListGamesResponse, ListGamesQuery, List<GameEntity>, ListGamesMapper>),
            typeof(ListGamesEndpoint).BaseType);
    }

    [Fact]
    public void Get_endpoint_contract_remains_request_response_pair()
    {
        Assert.Equal(
            typeof(BaseQueryEndpoint<GetGameRequest, GetGameResponse, GetGameQuery, GameEntity, GetGameMapper>),
            typeof(GetGameEndpoint).BaseType);
    }

    [Fact]
    public void Update_status_endpoint_contract_uses_mapper_based_endpoint()
    {
        Assert.Equal(
            typeof(BaseCommandEndpoint<UpdateGameStatusRequest, UpdateGameStatusResponse, UpdateGameStatusCommand, GameStatusUpdateResult, UpdateGameStatusMapper>),
            typeof(UpdateGameStatusEndpoint).BaseType);
    }

    [Fact]
    public void Endpoints_delegate_to_expected_dependencies()
    {
        var createCtor = typeof(CreateGameEndpoint).GetConstructors().Single();
        var getCtor = typeof(GetGameEndpoint).GetConstructors().Single();
        var listCtor = typeof(ListGamesEndpoint).GetConstructors().Single();
        var updateCtor = typeof(UpdateGameStatusEndpoint).GetConstructors().Single();

        Assert.Equal(
            new[] { typeof(ICreateGameHandler) },
            createCtor.GetParameters().Select(parameter => parameter.ParameterType).ToArray());
        Assert.Equal(
            new[] { typeof(IGetGameHandler) },
            getCtor.GetParameters().Select(parameter => parameter.ParameterType).ToArray());
        Assert.Equal(
            new[] { typeof(IListGamesHandler) },
            listCtor.GetParameters().Select(parameter => parameter.ParameterType).ToArray());
        Assert.Equal(
            new[] { typeof(IUpdateGameStatusHandler) },
            updateCtor.GetParameters().Select(parameter => parameter.ParameterType).ToArray());
    }
}
