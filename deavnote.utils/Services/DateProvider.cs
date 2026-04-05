namespace deavnote.utils.Services;

internal sealed class DateProvider : IDateProvider
{
    public DateOnly GetFirstDayOfWeek(DateTime from)
    {
        DateOnly today = DateOnly.FromDateTime(from);
        int daysFromMonday = (int)today.DayOfWeek - (int)DayOfWeek.Monday;
        if (daysFromMonday < 0)
        {
            daysFromMonday += 7;
        }

        return today.AddDays(-daysFromMonday);
    }

    public DateOnly GetFirstDayOfMonth(DateTime from)
    {
        DateOnly today = DateOnly.FromDateTime(from);
        return new DateOnly(today.Year, today.Month, 1);
    }

    public int GetDaysInMonth(DateTime from)
    {
        return DateTime.DaysInMonth(from.Year, from.Month);
    }
}
