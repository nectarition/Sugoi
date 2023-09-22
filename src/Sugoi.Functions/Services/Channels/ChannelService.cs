using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Sugoi.Functions.Services.Channels
{
    public interface IChannelService
    {
        Task<Models.Aggregates.Channel?> FindAsync(string channelId);

        Task SetLastMessageIdAsync(string channelId, string? lastMessageId);

        Task<Models.Aggregates.Channel[]> GetChannelsAsync();

        Task CreateChannelAsync(Models.Aggregates.Channel channel);
    }

    public class ChannelService : IChannelService
    {
        private CosmosClient CosmosClient { get; }

        public ChannelService(CosmosClient cosmosClient)
        {
            CosmosClient = cosmosClient;
        }

        public async Task<Models.Aggregates.Channel?> FindAsync(string channelId)
        {
            try
            {
                var channel = await CosmosClient.GetContainer("SugoiCosmosDb", "Channels")
                    .ReadItemAsync<Models.Aggregates.Channel>(channelId, new PartitionKey(channelId));
                return channel;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Models.Aggregates.Channel[]> GetChannelsAsync()
        {
            var channelIterator = CosmosClient.GetContainer("SugoiCosmosDb", "Channels")
                .GetItemLinqQueryable<Models.Aggregates.Channel>()
                .ToFeedIterator();

            var channels = new List<Models.Aggregates.Channel>();
            while (channelIterator.HasMoreResults)
            {
                foreach (var channel in await channelIterator.ReadNextAsync())
                {
                    channels.Add(channel);
                }
            }

            return channels.ToArray();
        }

        public async Task SetLastMessageIdAsync(string channelId, string? lastMessageId)
        {
            await CosmosClient.GetContainer("SugoiCosmosDb", "Channels")
                .PatchItemAsync<Models.Aggregates.Channel>(channelId, new PartitionKey(channelId), new[]
                {
                    PatchOperation.Set($"/lastMessageId", lastMessageId)
                });
        }

        public async Task CreateChannelAsync(Models.Aggregates.Channel channel)
        {
            await CosmosClient.GetContainer("SugoiCosmosDb", "Channels")
                .CreateItemAsync(channel);
        }
    }
}
