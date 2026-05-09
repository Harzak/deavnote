namespace deavnote.utils.Interfaces;

/// <summary>
/// Represents an action that can be executed with debounce behavior, ensuring that the action is not executed too
/// frequently.
/// </summary>
/// <remarks>This interface is typically used to wrap actions that should only be executed after a certain period
/// of inactivity or delay. Implementations may manage timing and state to enforce the debounce behavior.</remarks>
public interface IDebounceAction : IDisposable
{
    void Execute();
}
