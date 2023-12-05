namespace Sugoi.Functions.Helpers;

public class TimeHelper
{
    public const int SecondFormatLimit = 59;
    public const int MinuteFormatLimit = 59;
    public const int HourFormatLimit = 23;
    public const int DayFormatLimit = 6;
    public const int WeekFormatLimit = 3;
    public const int MonthFormatLimit = 12;

    public const int WeekDays = 7;
    public const int MonthDays = 30;
    public const int YearDays = 365;

    public static string FormatTimeSpan(TimeSpan ts)
    {
        if (ts.TotalSeconds <= SecondFormatLimit)
        {
            return $"{(int)ts.TotalSeconds} 秒前";
        }
        else if (ts.TotalMinutes <= MinuteFormatLimit)
        {
            return $"{(int)ts.TotalMinutes} 分前";
        }
        else if (ts.TotalHours <= HourFormatLimit)
        {
            return $"{(int)ts.TotalHours} 時間前";
        }
        else if (ts.TotalDays <= DayFormatLimit)
        {
            return $"{(int)ts.TotalDays} 日前";
        }

        var week = (int)Math.Floor(ts.TotalDays / WeekDays);
        var month = (int)Math.Floor(ts.TotalDays / MonthDays);

        if (week <= WeekFormatLimit)
        {
            return $"{week} 週間前";
        }
        else if (month <= MonthFormatLimit)
        {
            return $"{month} か月前";
        }
        else
        {
            var year = (int)Math.Floor(ts.TotalDays / YearDays);
            return $"{year} 年前";
        }
    }
}
