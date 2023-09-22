using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Sugoi.Functions.Services.Users;

public interface IUserService
{
    Task<Models.Aggregates.User?> FindAsync(string userId);
    Task<Models.Aggregates.User[]> GetUsersAsync();

    Task SetUserNameAsync(string userId, string userName);
    Task SetLastMessageAsync(string userId, string lastMessageId, string lastMessageChannelId, DateTime postedAt);

    Task CreateUserAsync(string userId);
    Task CreateUserAsync(Models.Aggregates.User user);

    Task DeleteUserAsync(string userId);
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

    public async Task<Models.Aggregates.User[]> GetUsersAsync()
    {
        var userIterator = CosmosClient.GetContainer("SugoiCosmosDb", "Users")
            .GetItemLinqQueryable<Models.Aggregates.User>()
            .ToFeedIterator();

        var users = new List<Models.Aggregates.User>();
        while (userIterator.HasMoreResults)
        {
            foreach (var user in await userIterator.ReadNextAsync())
            {
                users.Add(user);
            }
        }

        return users.ToArray();
    }

    public async Task SetUserNameAsync(string userId, string userName)
    {
        await CosmosClient.GetContainer("SugoiCosmosDb", "Users")
            .PatchItemAsync<Models.Aggregates.User>(userId, new PartitionKey(userId), new[]
            {
                PatchOperation.Set($"/userName", userName)
            });
    }

    public async Task SetLastMessageAsync(string userId, string lastMessageId, string lastMessageChannelId, DateTime postedAt)
    {
        await CosmosClient.GetContainer("SugoiCosmosDb", "Users")
            .PatchItemAsync<Models.Aggregates.User>(userId, new PartitionKey(userId), new[]
            {
                PatchOperation.Set("/lastMessageId", lastMessageId),
                PatchOperation.Set("/lastMessageChannelId", lastMessageChannelId),
                PatchOperation.Set("/postedAt", postedAt)
            });
    }

    public async Task CreateUserAsync(Models.Aggregates.User user)
        => await CreateUserCoreAsync(user);

    public async Task CreateUserAsync(string userId)
    {
        var user = new Models.Aggregates.User
        {
            Id = userId,
            UserId = userId,
            PostedAt = DateTime.UtcNow
        };

        await CreateUserCoreAsync(user);
    }

    private async Task CreateUserCoreAsync(Models.Aggregates.User user)
    {
        await CosmosClient.GetContainer("SugoiCosmosDb", "Users")
            .CreateItemAsync(user);
    }

    public async Task DeleteUserAsync(string userId)
    {
        await CosmosClient.GetContainer("SugoiCosmosDb", "Users")
            .DeleteItemAsync<Models.Aggregates.User>(userId, new PartitionKey(userId));
    }
}
