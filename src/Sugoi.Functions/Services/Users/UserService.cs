using Microsoft.Azure.Cosmos;

namespace Sugoi.Functions.Services.Users;

public interface IUserService
{
    Task<Models.Aggregates.User?> FindAsync(string userId);
}

public class UserService : IUserService
{
    private CosmosClient CosmosClient { get; }
    public UserService(CosmosClient cosmosClient)
    {
        CosmosClient = cosmosClient;
    }

    public async Task<Models.Aggregates.User?> FindAsync(string userId)
    {
        try
        {
            var user = await CosmosClient.GetContainer("SugoiCosmosDb", "Users")
                .ReadItemAsync<Models.Aggregates.User>(userId, new PartitionKey(userId));
            return user;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
