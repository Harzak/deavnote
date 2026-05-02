using System;
using System.Collections.Generic;
using System.Text;

namespace deavnote.utils.Extensions;

public static class StringExtensions
{
    public static bool EqualsOrdinalIgnoreCase(this string? value, string? other)
    {
        return string.Equals(value, other, StringComparison.OrdinalIgnoreCase);
    }
}
