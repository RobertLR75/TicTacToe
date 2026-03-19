using Service.Contracts.Requests;
using Service.Contracts.Responses;
using SharedLibrary.FastEndpoints;
using UserService.Features.Users.Endpoints.Get;
using UserService.Features.Users.Entities;

namespace UserService.Features.Users.Endpoints.Create;

public sealed class CreateUserEndpoint(ICreateUserHandler handler)
    : BaseCommandEndpoint<CreateUserRequest, UserModel, CreateUserCommand, UserEntity, CreateUserMapper>(handler)
{
    public override void Configure()
    {
        Post("/api/users");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Create a user";
            s.Description = "Creates a user and publishes a user created event.";
        });
    }

    protected override Task HandleResponseAsync(UserModel response, CancellationToken ct)
        => Send.CreatedAtAsync<GetUserEndpoint>(new { Id = response.UserId }, Response, cancellation: ct);
}
