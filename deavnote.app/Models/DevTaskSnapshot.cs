namespace deavnote.app.Models;

internal sealed class DevTaskSnapshot
{
    internal string Name { get; init; } = string.Empty;
    internal string Description { get; init; } = string.Empty;
    internal EDevTaskState State { get; init; }
    internal Version? Release { get; init; }
}

