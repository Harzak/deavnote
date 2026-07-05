namespace deavnote.utils.Extensions;

public static class StringExtensions
{
    public static bool EqualsOrdinalIgnoreCase(this string? value, string? other)
    {
        return string.Equals(value, other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Removes a single trailing newline sequence (\r\n or \n) from the string.
    /// </summary>
    public static string TrimTrailingNewLine(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.EndsWith("\r\n", StringComparison.Ordinal))
        {
            return value.Substring(0, value.Length - 2);
        }
        if (value.EndsWith('\n'))
        {
            return value.Substring(0, value.Length - 1);
        }
        return value;
    }

    /// <summary>
    /// Removes a single leading newline sequence (\r\n or \n) from the string.
    /// </summary>
    public static string TrimLeadingNewLine(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.StartsWith("\r\n", StringComparison.Ordinal))
        {
            return value.Substring(2);
        }
        if (value.StartsWith('\n'))
        {
            return value.Substring(1);
        }
        return value;
    }

    /// <summary>
    /// Removes single leading and trailing newline sequences from the string.
    /// </summary>
    public static string TrimSurroundingNewLines(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        string result = TrimLeadingNewLine(value);
        return TrimTrailingNewLine(result);
    }


    /// <summary>
    /// Finds the start of the line containing the specified index.
    /// Returns the index of the first character of that line.
    /// </summary>
    public static int FindLineStart(this string text, int index)
    {
        ArgumentNullException.ThrowIfNull(text);

        int position = index;
        while (position > 0 && text[position - 1] != '\n')
        {
            position--;
        }
        return position;
    }

    /// <summary>
    /// Finds the end of the line containing the specified index.
    /// Returns the index after the newline character(s), or the end of the string if no newline is found.
    /// </summary>
    public static int FindLineEnd(this string text, int index)
    {
        ArgumentNullException.ThrowIfNull(text);

        int position = index;
        while (position < text.Length && text[position] != '\n')
        {
            position++;
        }
        if (position < text.Length && text[position] == '\n')
        {
            position++;
        }
        return position;
    }
}
