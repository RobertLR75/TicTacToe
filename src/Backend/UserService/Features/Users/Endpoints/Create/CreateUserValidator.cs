using FastEndpoints;
using FluentValidation;
using Service.Contracts.Requests;

namespace UserService.Features.Users.Endpoints.Create;

public sealed class CreateUserValidator : Validator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must be 100 characters or fewer");
    }
}
