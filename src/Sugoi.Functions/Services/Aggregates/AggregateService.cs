using Microsoft.Extensions.Options;
using Sugoi.Functions.Configurations;
using Sugoi.Functions.Models.Aggregates;
using Sugoi.Functions.Models.Discord;
using Sugoi.Functions.Services.Channels;
using Sugoi.Functions.Services.Common;
using Sugoi.Functions.Services.Users;

namespace Sugoi.Functions.Services.Aggregates;

public interface IAggregateService
{
    Task AggregateMessages();
}

public class AggregateService : IAggregateService
{
    private SugoiConfiguration SugoiConfiguration { get; }
    private IDiscordService DiscordService { get; }
    private IChannelService ChannelService { get; }
    private IUserService UserService { get; }

    public AggregateService(
        IOptions<SugoiConfiguration> sugoiConfigurationOptions,
        IDiscordService discordService,
        IChannelService channelService,
        IUserService userService)
    {
        SugoiConfiguration = sugoiConfigurationOptions.Value;
        DiscordService = discordService;
        ChannelService = channelService;
        UserService = userService;
    }

    public async Task AggregateMessages()
    {
        var textChannels = (await DiscordService.GetChannelsAsync())
            .Where(c => c.DiscordChannelType == Enumerations.DiscordChannelTypes.GuildText);
        var expectedChannels = textChannels
            .Where(c => !SugoiConfiguration.Aggregates.ExceptParentIds.Contains(c.ParentId));
        var targetTextChannels = textChannels
            .Where(c => SugoiConfiguration.Aggregates.AllowChannelIds.Contains(c.Id));
        var selectedChannels = expectedChannels
            .Concat(targetTextChannels)
            .DistinctBy(c => c.Id)
            .ToArray();

        var fetchedChannels = await ChannelService.GetChannelsAsync();

        var fetchedMessages = new List<Message>();

        foreach (var channel in selectedChannels)
        {
            var fetchedChannel = fetchedChannels.FirstOrDefault(c => c.Id == channel.Id);
            var messages = fetchedChannel != null && !string.IsNullOrEmpty(fetchedChannel.LastMessageId)
                ? await DiscordService.GetMessagesAsync(channel.Id, fetchedChannel.LastMessageId)
                : await DiscordService.GetMessagesAsync(channel.Id);

            var userFilteredMessages = messages
                .OrderByDescending(m => m.CreatedAt)
                .DistinctBy(m => m.User.Id);
            fetchedMessages.AddRange(userFilteredMessages);
        }

        var users = fetchedMessages
            .GroupBy(m => m.User.Id)
            .Select(m =>
                {
                    var lastPostMessage = m.OrderByDescending(m => m.CreatedAt).First();
                    return new Models.Aggregates.User
                    {
                        Id = lastPostMessage.User.Id,
                        UserId = lastPostMessage.User.Id,
                        UserName = lastPostMessage.User.GlobalName,
                        PostedAt = lastPostMessage.CreatedAt,
                        LastMessageId = lastPostMessage.Id,
                        LastMessageChannelId = lastPostMessage.ChannelId
                    };
                })
            .ToArray();

        foreach (var user in users)
        {
            var fetchedUser = await UserService.FindAsync(user.Id);
            if (fetchedUser == null)
            {
                await UserService.CreateUserAsync(user);
                continue;
            }

            await UserService.SetLastMessageAsync(
                user.Id,
                user.LastMessageId,
                user.LastMessageChannelId,
                user.PostedAt);
        }

        foreach (var channel in selectedChannels)
        {
            var fetchedChannel = fetchedChannels.FirstOrDefault(c => c.Id == channel.Id);
            if (fetchedChannel == null)
            {
                var newChannel = new Channel
                {
                    Id = channel.Id,
                    ChannelId = channel.Id,
                    LastMessageId = channel.LastMessageId
                };

                await ChannelService.CreateChannelAsync(newChannel);
                continue;
            }

            await ChannelService.SetLastMessageIdAsync(channel.Id, channel.LastMessageId);
        }
    }
}
