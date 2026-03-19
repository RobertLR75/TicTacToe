using FastEndpoints;
using FluentValidation;
using Service.Contracts.Requests;

namespace UserService.Features.Users.Endpoints.Update;

public sealed class UpdateUserValidator : Validator<UpdateUserRequest>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must be 100 characters or fewer");
    }
}
