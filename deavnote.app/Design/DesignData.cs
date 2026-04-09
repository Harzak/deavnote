namespace deavnote.app.Design;

/// <summary>
/// Provides pre-populated ViewModel instances for the Avalonia designer.
/// </summary>
internal static class DesignData
{
    public static TimeEntryViewModel TimeEntryViewModel { get; } = new(new TimeEntry()
    {
        Name = "Design Time Entry",
        Code = "T123",
        Task = new DevTask()
        {
            Name = "Design Data Task",
            Code = "D456",
            CreatedAtUtc = DateTime.Now,
            Description = "This is a sample task used for design-time data in the Avalonia designer.",
            Note = "This task is only for design purposes and does not represent real data."
        },
        Duration = TimeSpan.FromHours(2)
    })
    {

    };
}