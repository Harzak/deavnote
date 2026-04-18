using deavnote.repository.Dto;
using deavnote.utils.Results;

[assembly: InternalsVisibleTo("deavnote.core.tests")]

namespace deavnote.core.Services;

/// <summary>
/// Manages time entry data, providing cursor-based access and change notifications for time entries within a specified
/// date and time range.
/// </summary>
/// <remarks>Use the DateCursor and DayOffset properties to control the current view window. Subscribe to the
/// TimeEntriesChanged event to be notified when the set of visible time entries changes.</remarks>
internal sealed class Journal : IJournal
{
    private readonly ITimeEntryRepository _repository;

    private readonly Dictionary<int, TimeEntry> _pool;
    private readonly List<TimeEntry> _entriesInCursor;
    private readonly HashSet<DateOnly> _fetchedDates;

    /// <inheritdoc/>
    public DateOnly DateCursor { get; private set; }
    /// <inheritdoc/>
    public int DayOffset { get; private set; }
    /// <inheritdoc/>
    public IReadOnlyCollection<TimeEntry> TimeEntries => _entriesInCursor.AsReadOnly();
    /// <inheritdoc/>
    public JournalConfiguration DefaultConfiguration { get; }

    /// <inheritdoc/>
    public event EventHandler<TimeEntriesChangedEventArgs>? TimeEntriesChanged;

    public event EventHandler<JournalCursorChangedEventArgs>? CursorChanged;

    public Journal(ITimeEntryRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);

        _repository = repository;
        _pool = [];
        _entriesInCursor = [];
        _fetchedDates = [];

        this.DefaultConfiguration = new JournalConfiguration
        {
            DateCursor = DateOnly.FromDateTime(DateTime.Today),
            DayOffset = 1
        };
    }

    /// <inheritdoc/>
    public async Task LoadDefaultCursorAsync(CancellationToken cancellationToken = default)
    {
        await this.SetCursorsAsync(this.DefaultConfiguration, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task SetCursorsAsync(JournalConfiguration configuration, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (this.TrySetDateCursor(configuration.DateCursor) | this.TrySetDayOffsetCursor(configuration.DayOffset))
        {
            await this.OnCursorChangedAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task ShiftDateCursorAsync(int days, CancellationToken cancellationToken = default)
    {
        await this.MoveDateCursorAsync(to: this.DateCursor.AddDays(days), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task ResetDateCursorAsync(CancellationToken cancellationToken = default)
    {
        await this.MoveDateCursorAsync(to: this.DefaultConfiguration.DateCursor, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<OperationResult> AddEntryAsync(AddTimeEntryRequest request, CancellationToken cancellationToken = default)
    {
        OperationResult result = await _repository.AddTimeEntryAsync(request, cancellationToken).ConfigureAwait(false);
        if (result.IsSuccess)
        {
            await this.LoadEntriesInCursorAsync(hardReload: true, cancellationToken).ConfigureAwait(false);
        }
        return result;
    }

    private async Task MoveDateCursorAsync(DateOnly to, CancellationToken cancellationToken = default)
    {
        if (this.TrySetDateCursor(to))
        {
            await this.OnCursorChangedAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private bool TrySetDateCursor(DateOnly date)
    {
        bool hasChanged = this.DateCursor != date;
        if (hasChanged)
        {
            this.DateCursor = date;
            this.InvokeCursorChanged();
        }
        return hasChanged;
    }

    private bool TrySetDayOffsetCursor(int time)
    {
        bool hasChanged = this.DayOffset != time;
        if (hasChanged)
        {
            this.DayOffset = time;
            this.InvokeCursorChanged();
        }
        return hasChanged;
    }

    private async Task OnCursorChangedAsync(CancellationToken cancellationToken = default)
    {
        await this.LoadEntriesInCursorAsync(hardReload: false, cancellationToken).ConfigureAwait(false);
        _ = this.LoadAdjacentEntriesAsync(this.DateCursor, this.DayOffset, cancellationToken);
    }

    private async Task LoadEntriesInCursorAsync(bool hardReload = false, CancellationToken cancellationToken = default)
    {
        DateOnly from = this.DateCursor;
        DateOnly to = this.DateCursor.AddDays(this.DayOffset);

        if (!_fetchedDates.Contains(from) || hardReload)
        {
            await this.LoadEntriesBetweenASync(from, to, cancellationToken).ConfigureAwait(false);
        }

        _entriesInCursor.Clear();
        IEnumerable<TimeEntry> entries = _pool.Values.Where(e => e.StartedAtUtc.IsInRangeExclusive(this.DateCursor, this.DateCursor.AddDays(this.DayOffset)));
        _entriesInCursor.AddRange(entries);

        this.InvokeTimeEntriesChanged();
    }

    private async Task LoadAdjacentEntriesAsync(DateOnly cursor, int dayOffset, CancellationToken cancellationToken = default)
    {
        DateOnly prevFrom = cursor.AddDays(-dayOffset);
        DateOnly nextFrom = cursor.AddDays(dayOffset);

        List<Task> prefetchTasks = [];

        if (!_fetchedDates.Contains(prevFrom))
        {
            DateOnly to = prevFrom.AddDays(dayOffset);
            Task previous = this.LoadEntriesBetweenASync(prevFrom, to, cancellationToken);
            prefetchTasks.Add(previous);
        }

        if (!_fetchedDates.Contains(nextFrom))
        {
            DateOnly to = nextFrom.AddDays(dayOffset);
            Task next = this.LoadEntriesBetweenASync(nextFrom, to, cancellationToken);
            prefetchTasks.Add(next);
        }

        await Task.WhenAll(prefetchTasks).ConfigureAwait(false);
    }

    private async Task LoadEntriesBetweenASync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TimeEntry> entries = await _repository.GetEntriesBetweenAsync(from, to, cancellationToken).ConfigureAwait(false);

        foreach (TimeEntry entry in entries)
        {
            _pool[entry.Id] = entry;
        }

        _fetchedDates.Add(from);
    }

    private void InvokeTimeEntriesChanged()
    {
        TimeEntriesChanged?.Invoke(this, new TimeEntriesChangedEventArgs(_entriesInCursor.Count));
    }

    private void InvokeCursorChanged()
    {
        CursorChanged?.Invoke(this, new JournalCursorChangedEventArgs(this.DateCursor, this.DayOffset));
    }

}

