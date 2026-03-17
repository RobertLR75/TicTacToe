using FastEndpoints;
using FluentValidation;
using Service.Contracts.Requests;

namespace GameStateService.Endpoints.GameStates.Update;



public class UpdateGameStateValidator : Validator<UpdateGameStateRequest>
{
    public UpdateGameStateValidator()
    {
        RuleFor(x => x.GameId).NotEmpty();
        RuleFor(x => x.Row).InclusiveBetween(0, 2);
        RuleFor(x => x.Col).InclusiveBetween(0, 2);
    }
}

