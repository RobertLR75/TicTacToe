using FastEndpoints;
using FluentValidation;
using GameService.Contracts;
using GameService.Models;

namespace GameService.Endpoints.Games.UpdateStatus;

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
