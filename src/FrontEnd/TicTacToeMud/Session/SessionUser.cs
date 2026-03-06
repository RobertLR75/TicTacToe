namespace TicTacToeMud.Session;

public readonly record struct SessionUser(Guid UserId, string Name);
