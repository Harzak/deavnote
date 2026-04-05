[assembly: InternalsVisibleTo("deavnote.app.tests")]

namespace deavnote.app.ViewModels;

internal sealed partial class JournalViewModel : BaseViewModel
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IJournal _journal;
    private readonly IDateProvider _dateProvider;

    private DateTime _dateCursor;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private ObservableCollection<TimeEntryViewModel> _timeEntries;

    public JournalViewModel(IJournal journal, IDateProvider dateProvider, IViewModelFactory viewModelFactory)
    {
        ArgumentNullException.ThrowIfNull(journal);
        ArgumentNullException.ThrowIfNull(dateProvider);
        ArgumentNullException.ThrowIfNull(viewModelFactory);

        _journal = journal;
        _dateProvider = dateProvider;
        _viewModelFactory = viewModelFactory;

        _journal.TimeEntriesChanged += OnJournalTimeEntriesChanged;
        _dateCursor = DateTime.Now.Date;
        _timeEntries = [];
    }

    public async Task InitializedAsync()
    {
        this.IsLoading = true;
        await _journal.LoadDefaultCursorAsync().ConfigureAwait(false);
        this.IsLoading = false;
    }

    private void OnJournalTimeEntriesChanged(object? sender, TimeEntriesChangedEventArgs e)
    {
        this.TimeEntries.Clear();

        if (_journal.TimeEntries.Count == 0) return;

        foreach (TimeEntry entry in _journal.TimeEntries)
        {
            TimeEntryViewModel timeEntryViewModel = _viewModelFactory.CreateTimeEntryViewModel(entry);
            this.TimeEntries.Add(timeEntryViewModel);
        }
    }

    [RelayCommand]
    private void AddTimeEntry()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private async Task MoveDateCursorToNowAsync()
    {
        await _journal.ResetDateCursorAsync().ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task MoveDateCursorToPreviousDayAsync()
    {
        await _journal.ShiftDateCursorAsync(days: -1).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task MoveDateCursorToNextDayAsync()
    {
        await _journal.ShiftDateCursorAsync(days: 1).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task ChangeJournalMode(EJournalMode mode)
    {
        JournalConfiguration configuration = mode switch
        {
            EJournalMode.Day => new JournalConfiguration()
            {
                DateCursor = DateOnly.FromDateTime(DateTime.Today),
                DayOffset = 1
            },
            EJournalMode.Week => new JournalConfiguration()
            {
                DateCursor = _dateProvider.GetFirstDayOfWeek(from: DateTime.Today),
                DayOffset = 7
            },
            EJournalMode.Month => new JournalConfiguration()
            {
                DateCursor = _dateProvider.GetFirstDayOfMonth(from: DateTime.Today),
                DayOffset = _dateProvider.GetDaysInMonth(from: DateTime.Today)
            },
            _ => _journal.DefaultConfiguration,
        };
        await _journal.SetCursorsAsync(configuration).ConfigureAwait(false);
    }

    [RelayCommand]
    private void CopyToClipboard()
    {
        throw new NotImplementedException();
    }
}