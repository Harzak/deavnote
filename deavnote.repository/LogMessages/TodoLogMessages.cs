namespace deavnote.repository.LogMessages;

internal static partial class TodoLogMessages
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Error updating Todo item with Id {todoId}")]
    internal static partial void LogFailedToUpdateTodo(ILogger logger, int todoId, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to add todo.")]
    internal static partial void LogFailedToAddTodo(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to delete completed todo items.")]
    internal static partial void LogFailedToDeleteCompletedTodos(ILogger logger, Exception exception);
}
