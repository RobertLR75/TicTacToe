using System.Net.Http;
using Microsoft.Azure.Cosmos;
using UserService.Services;

namespace UserService.Configuration;

public static class UserStorageServiceCollectionExtensions
{
    public static IServiceCollection AddUserStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<UserStorageOptions>()
            .Bind(configuration.GetSection(UserStorageOptions.SectionName));

        services.AddSingleton(_ =>
        {
            var connectionString = configuration.GetConnectionString("cosmos")
                ?? "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRG==;";

            return new CosmosClient(connectionString, new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway,
                LimitToEndpoint = true,
                HttpClientFactory = () => new HttpClient(new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }),
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            });
        });

        services.AddScoped<IUserStorageService, CosmosUserStorageService>();

        return services;
    }
}
