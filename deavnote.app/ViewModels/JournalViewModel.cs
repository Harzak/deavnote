[assembly: InternalsVisibleTo("deavnote.app.tests")]

namespace deavnote.app.ViewModels;

internal sealed partial class JournalViewModel : BaseViewModel
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IJournal _journal;
    private DateTime _dateCursor;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private ObservableCollection<TimeEntryViewModel> _timeEntries;

    public JournalViewModel(IJournal journal, IViewModelFactory viewModelFactory)
    {
        ArgumentNullException.ThrowIfNull(journal);
        ArgumentNullException.ThrowIfNull(viewModelFactory);

        _journal = journal;
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
        await _journal.ShiftDateCursorAsync(-1).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task MoveDateCursorToNextDayAsync()
    {
        await _journal.ShiftDateCursorAsync(1).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task ChangeAgendaMode(EAgendaMode mode)
    {
        JournalCursorsConfiguration configuration = mode switch
        {
            EAgendaMode.Day => new JournalCursorsConfiguration()
            {
                DateCursor = DateTime.Now.AddDays(1), //normalize
                TimeCursor = TimeSpan.FromDays(1)
            },
            EAgendaMode.Week => new JournalCursorsConfiguration()
            {
                DateCursor = DateTime.Now.AddDays(1), //normalize
                TimeCursor = TimeSpan.FromDays(7)
            },
            EAgendaMode.Month => new JournalCursorsConfiguration()
            {
                DateCursor = DateTime.Now.AddDays(1), //normalize
                TimeCursor = TimeSpan.FromDays(30)
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