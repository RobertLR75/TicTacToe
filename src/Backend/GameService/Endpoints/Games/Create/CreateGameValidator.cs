using FastEndpoints;
using FluentValidation;
using Service.Contracts.CreateGame;

namespace GameService.Endpoints.Games.Create;

public class CreateGameValidator : Validator<CreateGameRequest>
{
    public CreateGameValidator()
    {
        RuleFor(x => x.PlayerId)
            .NotEmpty().WithMessage("Player id is required");

        RuleFor(x => x.PlayerName)
            .NotEmpty().WithMessage("Player name is required")
            .MaximumLength(50).WithMessage("Player name must be 50 characters or fewer");
    }
}
