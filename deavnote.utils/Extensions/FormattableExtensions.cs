using System.Globalization;

namespace deavnote.utils.Extensions;

public static class FormattableExtensions
{
    public static string ToStringInvariant(this IFormattable value, string? format = null)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.ToString(format, CultureInfo.InvariantCulture);
    }
}