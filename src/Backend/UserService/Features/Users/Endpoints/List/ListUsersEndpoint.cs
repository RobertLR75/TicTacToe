using FastEndpoints;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using SharedLibrary.FastEndpoints;
using UserService.Features.Users.Entities;

namespace UserService.Features.Users.Endpoints.List;

public sealed class ListUsersEndpoint(IListUsersHandler handler)
    : BaseQueryEndpoint<ListUsersRequest, ListUsersResponse, ListUsersQuery, UserListEntity, ListUsersMapper>(handler)
{
    public override void Configure()
    {
        Get("/api/users");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "List users";
            s.Description = "Returns all users and serves the response from cache when available.";
        });
    }

}
