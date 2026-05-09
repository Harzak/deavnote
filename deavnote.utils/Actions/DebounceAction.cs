namespace deavnote.utils.Actions;

public sealed class DebounceAction : IDebounceAction
{
    private readonly Action _action;
    private readonly int _delayMs;
    private readonly Func<Action, Task>? _dispatcher;

    private Timer? _debounceTimer;

    public DebounceAction(Action action, int delayMs, Func<Action, Task>? dispatcher = null)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
        _delayMs = delayMs > 0 ? delayMs : throw new ArgumentOutOfRangeException(nameof(delayMs), "Delay must be greater than zero.");
        _dispatcher = dispatcher;
    }

    public void Execute()
    {
        _debounceTimer?.Dispose();
        _debounceTimer = new Timer(async _ =>
        {
            if (_dispatcher != null)
            {
                await _dispatcher(_action).ConfigureAwait(false);
            }
            else
            {
                _action();
            }
        },
        state: null,
        dueTime: _delayMs,
        period: Timeout.Infinite);
    }

    public void Dispose()
    {
        _debounceTimer?.Dispose();
        _debounceTimer = null;
    }
}

