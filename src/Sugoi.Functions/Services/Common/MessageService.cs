using Microsoft.Extensions.Logging;
using Sugoi.Functions.Models.Interactions;
using Sugoi.Functions.Services.Users;
using static Sugoi.Functions.Enumerations;

namespace Sugoi.Functions.Services.Common;

public interface IMessageService
{
    Task<InteractionResult> PongAsync(InteractionPayload payload);
    Task<InteractionResult> GetUserByIdAsync(InteractionPayload payload);
}

public class MessageService : IMessageService
{
    private ILogger Logger { get; }
    private IUserService UserService { get; }

    public MessageService(
        ILoggerFactory loggerFactory,
        IUserService userService)
    {
        Logger = loggerFactory.CreateLogger<MessageService>();
        UserService = userService;
    }

    public async Task<InteractionResult> PongAsync(InteractionPayload payload)
    {
        Logger.LogInformation("Pong!");

        return new InteractionResult
        {
            InteractionResponseType = InteractionResponseTypes.Pong
        };
    }

    public async Task<InteractionResult> GetUserByIdAsync(InteractionPayload payload)
    {
        var option = payload.Data?.Options.First();
        var userId = option?.Items.First(i => i.Name == "user").Value;
        Logger.LogInformation($"userId: {userId}");
        if (userId == null)
        {
            return new InteractionResult
            {
                InteractionResponseType = InteractionResponseTypes.ChannelMessageWithSoruce,
                Data = new()
                {
                    Content = "エラーが発生しました。"
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
                Content = $"ユーザId: {user.UserId}"
            }
        };
    }
}
