namespace deavnote.core.Configuration;

/// <summary>
/// Represents configuration settings for journal cursors, including date and time positions.
/// </summary>
public record JournalConfiguration
{
    /// <summary>
    /// Gets the current date and time cursor value.
    /// </summary>
    public required DateOnly DateCursor { get; init; }
    /// <summary>
    /// Gets the current position in time within the context of the operation.
    /// </summary>
    public required int DayOffset { get; init; }
}
