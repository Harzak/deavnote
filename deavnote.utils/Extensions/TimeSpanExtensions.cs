namespace deavnote.utils.Extensions;

public static class TimeSpanExtensions
{
    public static TimeSpan Sum(this IEnumerable<TimeSpan> source)
    {
        return TimeSpan.FromTicks(source.Sum(t => t.Ticks));
    }

    public static TimeSpan Sum<T>(this IEnumerable<T> source, Func<T, TimeSpan> selector)
    {
        return TimeSpan.FromTicks(source.Sum(x => selector(x).Ticks));
    }
}
