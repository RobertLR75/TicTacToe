using FastEndpoints;
using FluentValidation;
using Service.Contracts.Requests;
using Service.Contracts.Shared;

namespace UserService.Features.Users.Endpoints.UpdateStatus;

public sealed class UpdateUserStatusValidator : Validator<UpdateUserStatusRequest>
{
    public UpdateUserStatusValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Status)
            .Equal(UserStatusEnum.Disabled).WithMessage("Status must be Disabled");
    }
}
