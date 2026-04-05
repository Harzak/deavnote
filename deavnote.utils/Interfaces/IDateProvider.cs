namespace deavnote.utils.Interfaces;

public interface IDateProvider
{
    DateOnly GetFirstDayOfWeek(DateTime from);
    DateOnly GetFirstDayOfMonth(DateTime from);
    int GetDaysInMonth(DateTime from);
}