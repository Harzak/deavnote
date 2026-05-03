namespace deavnote.repository.LogMessages;

/// <summary>
/// Provides log message definitions for repository classes.
/// </summary>
internal static partial class RepositoryLogMessages
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to add time entry.")]
    internal static partial void LogFailedToAddTimeEntry(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to update time entry with ID {TimeEntryId}.")]
    internal static partial void LogFailedToUpdateTimeEntry(ILogger logger, int timeEntryId, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to update development task with ID {TaskId}.")]
    internal static partial void LogFailedToUpdateDevTask(ILogger logger, int taskId, Exception exception);
}
