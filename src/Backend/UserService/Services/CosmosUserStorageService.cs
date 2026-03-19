using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using UserService.Configuration;
using DomainUser = UserService.Features.Users.Entities.UserEntity;
using DomainUserStatus = UserService.Features.Users.Entities.UserStatus;

namespace UserService.Services;

public sealed class CosmosUserStorageService(
    CosmosClient cosmosClient,
    IOptions<UserStorageOptions> storageOptions) : IUserStorageService
{
    private readonly CosmosClient _cosmosClient = cosmosClient;
    private readonly UserStorageOptions _storageOptions = storageOptions.Value;
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private Container? _container;

    public async Task<DomainUser> CreateAsync(DomainUser user, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        var container = await GetContainerAsync(ct);
        var document = UserDocument.FromDomain(user);

        await container.CreateItemAsync(document, new PartitionKey(document.Id), cancellationToken: ct);
        return user;
    }

    public async Task UpdateAsync(DomainUser user, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        var container = await GetContainerAsync(ct);
        var document = UserDocument.FromDomain(user);

        await container.UpsertItemAsync(document, new PartitionKey(document.Id), cancellationToken: ct);
    }

    public async Task<DomainUser?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var container = await GetContainerAsync(ct);

        try
        {
            var response = await container.ReadItemAsync<UserDocument>(id.ToString("D"), new PartitionKey(id.ToString("D")), cancellationToken: ct);
            return response.Resource.ToDomain();
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IReadOnlyList<DomainUser>> ListAsync(CancellationToken ct = default)
    {
        var container = await GetContainerAsync(ct);
        var iterator = container.GetItemQueryIterator<UserDocument>(
            new QueryDefinition("SELECT * FROM c ORDER BY c.createdAt"));

        var users = new List<DomainUser>();

        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync(ct);
            users.AddRange(page.Select(document => document.ToDomain()));
        }

        return users;
    }

    private async Task<Container> GetContainerAsync(CancellationToken ct)
    {
        if (_container is not null)
            return _container;

        await _initializationLock.WaitAsync(ct);

        try
        {
            if (_container is not null)
                return _container;

            var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_storageOptions.DatabaseName, cancellationToken: ct);
            var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(
                new ContainerProperties(_storageOptions.ContainerName, _storageOptions.PartitionKeyPath),
                cancellationToken: ct);

            _container = containerResponse.Container;
            return _container;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    private sealed class UserDocument
    {
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        public required string Name { get; init; }

        public required string Status { get; init; }

        public required DateTimeOffset CreatedAt { get; init; }

        public DateTimeOffset? UpdatedAt { get; init; }

        public static UserDocument FromDomain(DomainUser user) => new()
        {
            Id = user.Id.ToString("D"),
            Name = user.Name,
            Status = user.Status.ToString(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        public DomainUser ToDomain() => new()
        {
            Id = Guid.Parse(Id),
            Name = Name,
            Status = Enum.Parse<DomainUserStatus>(Status),
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt
        };
    }
}
