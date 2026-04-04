namespace deavnote.model.Enums;

/// <summary>
/// Specifies the possible states of a development task.
/// </summary>
public enum EDevTaskState
{
    Unknown = 0,
    NotStarted = 1,
    InProgress = 2,
    Completed = 3,
    Merged = 4,
    Tested = 5,
    Rejected = 6,
}
