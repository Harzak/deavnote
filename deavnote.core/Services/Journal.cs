[assembly: InternalsVisibleTo("deavnote.core.tests")]

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
    public JournalCursorsConfiguration DefaultConfiguration { get; }

    /// <inheritdoc/>
    public event EventHandler<TimeEntriesChangedEventArgs>? TimeEntriesChanged;

    public Journal(ITimeEntryRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);

        _repository = repository;
        _pool = [];
        _entriesInCursor = [];
        _poolIds = [];

        this.DefaultConfiguration = new JournalCursorsConfiguration
        {
            DateCursor = DateTime.Now.Date,
            TimeCursor = TimeSpan.FromDays(1)
        };
    }

    /// <inheritdoc/>
    public async Task LoadDefaultCursorAsync()
    {
        await this.SetCursorsAsync(this.DefaultConfiguration).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task SetCursorsAsync(JournalCursorsConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (this.TrySetDateCursor(configuration.DateCursor) | this.TrySetTimeCursor(configuration.TimeCursor))
        {
            await this.OnCursorChangedAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task ShiftDateCursorAsync(int days)
    {
        if (this.TrySetDateCursor(this.DateCursor.AddDays(days)))
        {
            await this.OnCursorChangedAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task ResetDateCursorAsync()
    {
        if (this.TrySetDateCursor(this.DefaultConfiguration.DateCursor))
        {
            await this.OnCursorChangedAsync().ConfigureAwait(false);
        }
    }

    private bool TrySetDateCursor(DateTime date)
    {
        bool hasChanged = this.DateCursor != date;
        if (hasChanged)
        {
            this.DateCursor = date;
        }
        return hasChanged;
    }

    private bool TrySetTimeCursor(TimeSpan time)
    {
        bool hasChanged = this.TimeCursor != time;
        if (hasChanged)
        {
            this.TimeCursor = time;
        }
        return hasChanged;
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

