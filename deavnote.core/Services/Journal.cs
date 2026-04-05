namespace deavnote.core.Services;

/// <summary>
/// Manages time entry data, providing cursor-based access and change notifications for time entries within a specified
/// date and time range.
/// </summary>
/// <remarks>Use the DateCursor and TimeCursor properties to control the current view window. Subscribe to the
/// TimeEntriesChanged event to be notified when the set of visible time entries changes.</remarks>
internal sealed class Journal : IJournal
{
    private readonly ITimeEntryRepository _repository;

    private readonly HashSet<int> _poolIds;
    private readonly List<TimeEntry> _pool;
    private readonly List<TimeEntry> _entriesInCursor;

    /// <inheritdoc/>
    public DateTime DateCursor { get; private set; }
    /// <inheritdoc/>
    public TimeSpan TimeCursor { get; private set; }
    /// <inheritdoc/>
    public IReadOnlyCollection<TimeEntry> TimeEntries => _entriesInCursor.AsReadOnly();

    /// <inheritdoc/>
    public event EventHandler<TimeEntriesChangedEventArgs>? TimeEntriesChanged;

    public Journal(ITimeEntryRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);

        _repository = repository;
        _pool = [];
        _entriesInCursor = [];
        _poolIds = [];
    }

    /// <inheritdoc/>
    public async Task LoadDefaultCursorAsync()
    {
        this.DateCursor = DateTime.Now.Date;
        this.TimeCursor = TimeSpan.FromDays(1);
        await this.OnCursorChangedAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task SetCursorsAsync(DateTime date, TimeSpan time)
    {
        this.DateCursor = date;
        this.TimeCursor = time;
        await this.OnCursorChangedAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task SetDateCursorAsync(DateTime date)
    {
        if (this.DateCursor != date)
        {
            this.DateCursor = date;
            await OnCursorChangedAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task SetTimeCursorAsync(TimeSpan time)
    {
        if (this.TimeCursor != time)
        {
            this.TimeCursor = time;
            await OnCursorChangedAsync().ConfigureAwait(false);
        }
    }

    private async Task OnCursorChangedAsync()
    {
        await this.LoadEntriesInCursorAsync().ConfigureAwait(false);

        _entriesInCursor.Clear();
        IEnumerable<TimeEntry> entries = _pool.Where(e => e.StartedAtUtc >= this.DateCursor
                                                     && e.StartedAtUtc < this.DateCursor.Add(this.TimeCursor));
        _entriesInCursor.AddRange(entries);

        TimeEntriesChanged?.Invoke(this, new TimeEntriesChangedEventArgs(this.DateCursor, this.TimeCursor));
    }

    private async Task LoadEntriesInCursorAsync()
    {
        DateTime from = this.DateCursor;
        DateTime to = this.DateCursor.Add(TimeCursor);

        await this.LoadEntriesBetweenAsync(from, to).ConfigureAwait(false);
    }

    private async Task LoadEntriesBetweenAsync(DateTime fromUtc, DateTime toUtc)
    {
        IReadOnlyList<TimeEntry> entries = await _repository.GetEntriesBetween(fromUtc, toUtc).ConfigureAwait(false);

        foreach (TimeEntry entry in entries)
        {
            if (_poolIds.Add(entry.Id))
            {
                _pool.Add(entry);
            }
        }
    }
}

