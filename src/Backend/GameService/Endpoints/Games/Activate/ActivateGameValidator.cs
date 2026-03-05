using FastEndpoints;
using FluentValidation;

namespace GameService.Endpoints.Games.Activate;

public class ActivateGameValidator : Validator<ActivateGameRequest>
{
    public ActivateGameValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Game ID is required");

        RuleFor(x => x.PlayerName)
            .NotEmpty().WithMessage("Player name is required")
            .MaximumLength(50).WithMessage("Player name must be 50 characters or fewer");
    }
}
