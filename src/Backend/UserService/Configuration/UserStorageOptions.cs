namespace UserService.Configuration;

public sealed class UserStorageOptions
{
    public const string SectionName = "UserStorage";

    public string DatabaseName { get; set; } = "tictactoe-users";

    public string ContainerName { get; set; } = "users";

    public string PartitionKeyPath { get; set; } = "/id";
}
