namespace deavnote.app.Design;

/// <summary>
/// Provides pre-populated ViewModel instances for the Avalonia designer.
/// </summary>
internal static class DesignData
{
    public static TimeEntryListItemViewModel TimeEntryViewModel { get; } = new(new TimeEntry()
    {
        Name = "Design Time Entry",
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

    public static TimeEntryDetailViewModel TimeEntryDetailViewModel { get; } = new(new TimeEntry()
    {
        Name = "Design Time Entry Detail",
        WorkDone = "Worked on design-time data for the Avalonia designer.",
        Duration = TimeSpan.FromHours(3),
        StartedAtUtc = DateTime.Now.AddHours(-3),
        CreatedAtUtc = DateTime.Now.AddDays(-1),
        UpdatedAtUtc = DateTime.Now
    })
    {
    };

    public static DevTaskDetailViewModel DevTaskDetailViewModel { get; } = new(new DevTask()
    {
        Name = "Design Dev Task Detail",
        Code = "D789",
        Description = "This is a sample development task used for design-time data in the Avalonia designer.",
        Note = "This task is only for design purposes and does not represent real data.",
        State = EDevTaskState.InProgress,
        CreatedAtUtc = DateTime.Now.AddDays(-2),
        UpdatedAtUtc = DateTime.Now
    })
    {
    };
}