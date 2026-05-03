using System;
using System.Collections.Generic;
using System.Text;

namespace deavnote.repository.LogMessages;

internal static partial class TodoLogMessages
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Error updating Todo item with Id {todoId}")]
    internal static partial void LogFailedToUpdateTodo(ILogger logger, int todoId, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to add todo.")]
    internal static partial void LogFailedToAddTodo(ILogger logger, Exception exception);
}
