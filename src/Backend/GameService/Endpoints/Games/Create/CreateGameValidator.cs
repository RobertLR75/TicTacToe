using FastEndpoints;
using FluentValidation;

namespace GameService.Endpoints.Games.Create;

public class CreateGameValidator : Validator<CreateGameRequest>
{
    public CreateGameValidator()
    {
        RuleFor(x => x.PlayerName)
            .NotEmpty().WithMessage("Player name is required")
            .MaximumLength(50).WithMessage("Player name must be 50 characters or fewer");
    }
}
