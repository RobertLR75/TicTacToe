using FastEndpoints;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using SharedLibrary.FastEndpoints;
using UserService.Features.Users.Endpoints.Create;
using UserService.Features.Users.Endpoints.Get;
using UserService.Features.Users.Endpoints.List;
using UserService.Features.Users.Entities;
using UserService.Features.Users.Endpoints.Update;
using UserService.Features.Users.Endpoints.UpdateStatus;
using Xunit;

namespace UserService.UnitTests;

public sealed class EndpointParityUnitTests
{
    [Fact]
    public void Create_endpoint_contract_remains_request_response_pair()
    {
        Assert.Equal(typeof(BaseCommandEndpoint<CreateUserRequest, UserModel, CreateUserCommand, UserEntity, CreateUserMapper>), typeof(CreateUserEndpoint).BaseType);
    }

    [Fact]
    public void Update_endpoint_contract_remains_request_response_pair()
    {
        Assert.Equal(typeof(BaseCommandEndpoint<UpdateUserRequest, UserModel, UpdateUserCommand, UserEntity, UpdateUserMapper>), typeof(UpdateUserEndpoint).BaseType);
    }

    [Fact]
    public void Get_endpoint_contract_remains_request_response_pair()
    {
        Assert.Equal(typeof(BaseQueryEndpoint<GetUserRequest, UserModel, GetUserQuery, UserEntity, GetUserMapper>), typeof(GetUserEndpoint).BaseType);
    }

    [Fact]
    public void List_endpoint_contract_remains_request_response_pair()
    {
        Assert.Equal(typeof(BaseQueryEndpoint<ListUsersRequest, ListUsersResponse, ListUsersQuery, UserListEntity, ListUsersMapper>), typeof(ListUsersEndpoint).BaseType);
    }

    [Fact]
    public void Update_status_endpoint_contract_remains_request_response_pair()
    {
        Assert.Equal(typeof(BaseCommandEndpoint<UpdateUserStatusRequest, UpdateUserStatusResponse, UpdateUserStatusCommand, UpdateUserStatusResult, UpdateUserStatusMapper>), typeof(UpdateUserStatusEndpoint).BaseType);
    }
}
