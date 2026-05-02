namespace deavnote.model.Enums;

/// <summary>
/// Specifies the available modes for displaying the journal.
/// </summary>
public enum EJournalMode
{
    /// <summary>
    /// Represents a time entry for a specific day.
    /// </summary>
    [Display(Name = "Single Entry for the Day")]
    TimeEntry,
    /// <summary>
    /// Represents a collection of time entries for a specific day.
    /// </summary>
    [Display(Name = "All Entries for the Day")]
    Day,
    /// <summary>
    /// Represents a collection of time entries for a specific week.
    /// </summary>
    [Display(Name = "All Entries for the Week")]
    Week,
    /// <summary>
    /// Represents a collection of time entries for a specific month.
    /// </summary>
    [Display(Name = "All Entries for the Month")]
    Month,
}