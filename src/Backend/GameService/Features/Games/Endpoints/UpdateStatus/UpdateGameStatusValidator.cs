using FastEndpoints;
using FluentValidation;
using Service.Contracts.Requests;
using Service.Contracts.Shared;

namespace GameService.Features.Games.Endpoints.UpdateStatus;

public sealed class UpdateGameStatusValidator : Validator<UpdateGameStatusRequest>
{
    public UpdateGameStatusValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Game ID is required");

        RuleFor(x => x.Status)
            .Must(status => status is GameStatusEnum.Active or GameStatusEnum.Completed)
            .WithMessage("Status must be Active or Completed");
    }
}
