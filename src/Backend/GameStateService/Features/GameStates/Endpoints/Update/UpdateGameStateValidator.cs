using FastEndpoints;
using FluentValidation;
using Service.Contracts.Requests;

namespace GameStateService.Features.GameStates.Endpoints.Update;



public sealed class UpdateGameStateValidator : Validator<UpdateGameStateRequest>
{
    public UpdateGameStateValidator()
    {
        RuleFor(x => x.GameId).NotEmpty();
        RuleFor(x => x.Row).InclusiveBetween(0, 2);
        RuleFor(x => x.Col).InclusiveBetween(0, 2);
    }
}
