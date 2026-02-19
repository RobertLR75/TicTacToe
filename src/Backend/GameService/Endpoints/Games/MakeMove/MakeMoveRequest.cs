using FastEndpoints;
using FluentValidation;

namespace GameService.Endpoints.Games.MakeMove;

public class MakeMoveRequest
{
    public required string GameId { get; init; }
    public required int Row { get; init; }
    public required int Col { get; init; }

    public class MakeMoveValidator : Validator<MakeMoveRequest>
    {
        public MakeMoveValidator()
        {
            RuleFor(x => x.GameId).NotEmpty();
            RuleFor(x => x.Row).InclusiveBetween(0, 2);
            RuleFor(x => x.Col).InclusiveBetween(0, 2);
        }
    }
}

