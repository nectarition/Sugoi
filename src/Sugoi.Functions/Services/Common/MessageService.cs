using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sugoi.Functions.Configurations;
using Sugoi.Functions.Helpers;
using Sugoi.Functions.Models.Interactions;
using Sugoi.Functions.Services.Users;
using static Sugoi.Functions.Enumerations;

namespace Sugoi.Functions.Services.Common;

public interface IMessageService
{
    Task<InteractionResult> PongAsync();
    Task<InteractionResult> GetUserByIdAsync(InteractionDataOption option);
    Task<InteractionResult> SetUserNameAsync(InteractionDataOption option);
    Task<InteractionResult> CreateUserAsync(InteractionDataOption option);
    Task<InteractionResult> DeleteUserAsync(InteractionDataOption option);
    Task<InteractionResult> GetAggregateResult();
}

public class MessageService : IMessageService
{
    private SugoiConfiguration SugoiConfiguration { get; }
    private ILogger Logger { get; }
    private IUserService UserService { get; }

    public MessageService(
        IOptions<SugoiConfiguration> configuration,
        ILoggerFactory loggerFactory,
        IUserService userService)
    {
        SugoiConfiguration = configuration.Value;
        Logger = loggerFactory.CreateLogger<MessageService>();
        UserService = userService;
    }

    public async Task<InteractionResult> PongAsync()
    {
        Logger.LogInformation("Pong!");

        return new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.Pong
        };
    }

    public async Task<InteractionResult> GetUserByIdAsync(InteractionDataOption option)
    {
        var userId = option.Items.First(i => i.Name == "user")?.Value;
        if (userId == null)
        {
            return new InteractionResult
            {
                InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
                Data = new()
                {
                    Content = "ユーザIDを取得できませんでした。"
                }
            };
        }

        var user = await UserService.FindAsync(userId);
        if (user == null)
        {
            return new InteractionResult
            {
                InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
                Data = new()
                {
                    Content = "ユーザ情報が見つかりませんでした。"
                }
            };
        }

        return new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
            Data = new()
            {
                Content = UserHelper.FormatUser(SugoiConfiguration.Aggregates.GuildId, user)
            }
        };
    }

    public async Task<InteractionResult> SetUserNameAsync(InteractionDataOption option)
    {
        var userId = option.Items.FirstOrDefault(i => i.Name == "user")?.Value;
        if (userId == null)
        {
            return new InteractionResult
            {
                InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
                Data = new()
                {
                    Content = "ユーザIDを取得できませんでした。"
                }
            };
        }

        var userName = option.Items.FirstOrDefault(i => i.Name == "name")?.Value;
        if (userName == null)
        {
            return new InteractionResult
            {
                InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
                Data = new()
                {
                    Content = "ユーザ名を取得できませんでした。"
                }
            };
        }

        await UserService.SetUserNameAsync(userId, userName);

        return new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
            Data = new()
            {
                Content = $"表示名を `{userName}` に変更しました。"
            }
        };
    }

    public async Task<InteractionResult> CreateUserAsync(InteractionDataOption option)
    {
        var userId = option.Items.FirstOrDefault(i => i.Name == "user")?.Value;
        if (userId == null)
        {
            return new InteractionResult
            {
                InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
                Data = new()
                {
                    Content = "ユーザIDを取得できませんでした。"
                }
            };
        }

        await UserService.CreateUserAsync(userId);

        return new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
            Data = new()
            {
                Content = "集計用ユーザ情報を作成しました。"
            }
        };
    }

    public async Task<InteractionResult> DeleteUserAsync(InteractionDataOption option)
    {
        var userId = option.Items.FirstOrDefault(i => i.Name == "user")?.Value;
        if (userId == null)
        {
            return new InteractionResult
            {
                InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
                Data = new()
                {
                    Content = "ユーザIDを取得できませんでした。"
                }
            };
        }

        await UserService.DeleteUserAsync(userId);

        return new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
            Data = new()
            {
                Content = "集計用ユーザ情報を削除しました。"
            }
        };
    }

    public async Task<InteractionResult> GetAggregateResult()
    {
        var users = await UserService.GetUsersAsync();
        if (!users.Any())
        {
            return new InteractionResult
            {
                InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
                Data = new()
                {
                    Content = "集計情報を取得できませんでした。"
                }
            };
        }

        var prefix = "----- 最新の集計情報\n";

        var userMessages = users
            .OrderByDescending(u => u.PostedAt)
            .Select(user => UserHelper.FormatSimpleUser(SugoiConfiguration.Aggregates.GuildId, user));
        var userMessage = string.Join('\n', userMessages);

        return new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
            Data = new()
            {
                Content = $"{prefix}{userMessage}"
            }
        };
    }
}
