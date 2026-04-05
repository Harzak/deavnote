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

    private readonly Dictionary<int, TimeEntry> _pool;
    private readonly List<TimeEntry> _entriesInCursor;
    private readonly HashSet<DateTime> _fetchedDates;

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
        _fetchedDates = [];

        this.DefaultConfiguration = new JournalCursorsConfiguration
        {
            DateCursor = DateTime.Now.Date,
            TimeCursor = TimeSpan.FromDays(1)
        };
    }

    /// <inheritdoc/>
    public async Task LoadDefaultCursorAsync(CancellationToken cancellationToken = default)
    {
        await this.SetCursorsAsync(this.DefaultConfiguration, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task SetCursorsAsync(JournalCursorsConfiguration configuration, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (this.TrySetDateCursor(configuration.DateCursor) | this.TrySetTimeCursor(configuration.TimeCursor))
        {
            await this.OnCursorChangedAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task ShiftDateCursorAsync(int days, CancellationToken cancellationToken = default)
    {
        await this.MoveDateCursorAsync(this.DateCursor.AddDays(days), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task ResetDateCursorAsync(CancellationToken cancellationToken = default)
    {
        await this.MoveDateCursorAsync(this.DefaultConfiguration.DateCursor, cancellationToken).ConfigureAwait(false);
    }

    private async Task MoveDateCursorAsync(DateTime to, CancellationToken cancellationToken = default)
    {
        if (this.TrySetDateCursor(to))
        {
            await this.OnCursorChangedAsync(cancellationToken).ConfigureAwait(false);
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

    private async Task OnCursorChangedAsync(CancellationToken cancellationToken = default)
    {
        await this.LoadEntriesInCursorAsync(cancellationToken).ConfigureAwait(false);

        _entriesInCursor.Clear();
        IEnumerable<TimeEntry> entries = _pool.Values.Where(e => e.StartedAtUtc >= this.DateCursor
                                                            && e.StartedAtUtc < this.DateCursor.Add(this.TimeCursor));

        if (entries.Any())
        {
            _entriesInCursor.AddRange(entries);

            TimeEntriesChanged?.Invoke(this, new TimeEntriesChangedEventArgs(this.DateCursor, this.TimeCursor));
        }

        _ = this.LoadAdjacentEntriesAsync(this.DateCursor, this.TimeCursor, cancellationToken);
    }

    private async Task LoadEntriesInCursorAsync(CancellationToken cancellationToken = default)
    {
        DateTime from = this.DateCursor;
        DateTime to = this.DateCursor.Add(this.TimeCursor);

        if (_fetchedDates.Contains(from))
        {
            return;
        }

        await this.LoadEntriesBetweenASync(from, to, cancellationToken).ConfigureAwait(false);
    }

    private async Task LoadAdjacentEntriesAsync(DateTime cursor, TimeSpan timeCursor, CancellationToken cancellationToken = default)
    {
        DateTime prevFrom = cursor.Add(-timeCursor);
        DateTime nextFrom = cursor.Add(timeCursor);

        List<Task> prefetchTasks = [];

        if (!_fetchedDates.Contains(prevFrom))
        {
            DateTime to = prevFrom.Add(timeCursor);
            prefetchTasks.Add(this.LoadEntriesBetweenASync(prevFrom, to, cancellationToken));
        }

        if (!_fetchedDates.Contains(nextFrom))
        {
            DateTime to = nextFrom.Add(timeCursor);
            prefetchTasks.Add(this.LoadEntriesBetweenASync(nextFrom, to, cancellationToken));
        }

        await Task.WhenAll(prefetchTasks).ConfigureAwait(false);
    }

    private async Task LoadEntriesBetweenASync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TimeEntry> entries = await _repository.GetEntriesBetween(from, to, cancellationToken).ConfigureAwait(false);

        foreach (TimeEntry entry in entries)
        {
            _pool[entry.Id] = entry;
        }

        _fetchedDates.Add(from);
    }
}

