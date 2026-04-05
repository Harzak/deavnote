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

    public static bool IsInRange(this DateTime date, DateOnly from, DateOnly to)
    {
        DateOnly day = DateOnly.FromDateTime(date);
        return day >= from && day <= to;
    }
}