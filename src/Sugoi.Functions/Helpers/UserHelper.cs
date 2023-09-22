namespace Sugoi.Functions.Helpers;

public class UserHelper
{
    public static string FormatUser(Models.Aggregates.User user)
    {
        var now = DateTime.Now;
        var span = now - user.PostedAt;
        var formatedSpan = TimeHelper.FormatTimeSpan(span);

        return $"ユーザId: {user.UserId}\n"
            + $"表示名: {user.UserName ?? "未登録"}\n"
            + $"最終投稿日: {user.PostedAt} ({formatedSpan})";
    }

    public static string FormatSimpleUser(Models.Aggregates.User user)
    {
        var now = DateTime.Now;
        var span = now - user.PostedAt;
        var formatedSpan = TimeHelper.FormatTimeSpan(span);

        return $"**{user.UserName ?? "誰？"}** {formatedSpan} || {user.Id} / {user.PostedAt:yyyy/MM/dd HH:mm:ss} ||";
    }
}
