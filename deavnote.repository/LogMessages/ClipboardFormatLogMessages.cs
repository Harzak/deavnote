namespace deavnote.repository.LogMessages;

/// <summary>
/// Provides log message definitions for clipboard format repository classes.
/// </summary>
internal static partial class ClipboardFormatLogMessages
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to update clipboard format for context '{Context}'.")]
    internal static partial void LogFailedToUpdateClipboardFormat(ILogger logger, EJournalMode context, Exception exception);
}
