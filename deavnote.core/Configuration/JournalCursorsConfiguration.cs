namespace deavnote.core.Configuration;

/// <summary>
/// Represents configuration settings for journal cursors, including date and time positions.
/// </summary>
public record JournalCursorsConfiguration
{
    /// <summary>
    /// Gets the current date and time cursor value.
    /// </summary>
    public required DateTime DateCursor { get; init; }
    /// <summary>
    /// Gets the current position in time within the context of the operation.
    /// </summary>
    public required TimeSpan TimeCursor { get; init; }
}
