namespace Sugoi.Functions.Helpers;

public class UserHelper
{

    private const string DiscordMessageURL = "https://discord.com/channels/{0}/{1}/{2}";

    public static string FormatUser(Models.Aggregates.User user)
    {
        var now = DateTime.Now;
        var span = now - user.PostedAt;
        var formatedSpan = TimeHelper.FormatTimeSpan(span);

        return $"ユーザId: {user.UserId}\n"
            + $"表示名: {user.UserName ?? "未登録"}\n"
            + $"最終投稿日: {user.PostedAt} ({formatedSpan})";
    }

    public static string FormatSimpleUser(string guildId, Models.Aggregates.User user)
    {
        var now = DateTime.Now;
        var span = now - user.PostedAt;
        var formatedSpan = TimeHelper.FormatTimeSpan(span);

        var discordMessageUrl = string.Format(DiscordMessageURL, guildId, user.LastMessageChannelId, user.LastMessageId);

        return $"**{user.UserName ?? "誰？"}**\n"
            + $"… {formatedSpan} {discordMessageUrl}";
    }
}
