using Service.Contracts.Requests;
using Service.Contracts.Responses;
using SharedLibrary.FastEndpoints;
using UserService.Features.Users.Entities;

namespace UserService.Features.Users.Endpoints.Get;

public sealed class GetUserEndpoint(IGetUserHandler handler)
    : BaseQueryEndpoint<GetUserRequest, UserModel, GetUserQuery, UserEntity, GetUserMapper>(handler)
{
    public override void Configure()
    {
        Get("/api/users/{Id}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get a user by id";
            s.Description = "Returns one user and serves the response from cache when available.";
        });
    }

}
