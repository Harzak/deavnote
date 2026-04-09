namespace deavnote.utils.Extensions;

public static class DateTimeExtensions
{
    public static bool IsInDay(this DateTime date, DateOnly day)
    {
        return DateOnly.FromDateTime(date) == day;
    }

    public static bool IsInMonth(this DateTime date, DateOnly month)
    {
         return date.Year == month.Year && date.Month == month.Month;
    }

    /// <summary>
    /// Returns true if the date falls within [from, to], where <paramref name="to"/> is inclusive
    /// and covers the entire day (i.e., any time on the <paramref name="to"/> date is included).
    /// </summary>
    public static bool IsInRange(this DateTime date, DateOnly from, DateOnly to)
    {
        DateOnly day = DateOnly.FromDateTime(date);
        return day >= from && day <= to;
    }

    /// <summary>
    /// Returns true if the date falls within [from, to), where <paramref name="to"/> is exclusive
    /// (i.e., a moment at 08:00 on the <paramref name="to"/> date is NOT included).
    /// </summary>
    public static bool IsInRangeExclusive(this DateTime date, DateOnly from, DateOnly to)
    {
        DateOnly day = DateOnly.FromDateTime(date);
        return day >= from && day < to;
    }
}