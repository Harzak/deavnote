using deavnote.app.ViewModels.Base;

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
        _dateCursor = DateTime.Now.Date;
        await _journal.SetDateCursorAsync(_dateCursor).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task MoveDateCursorToPreviousDayAsync()
    {
        _dateCursor = _dateCursor.AddDays(-1);
        await _journal.SetDateCursorAsync(_dateCursor).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task MoveDateCursorToNextDayAsync()
    {
        _dateCursor = _dateCursor.AddDays(1);
        await _journal.SetDateCursorAsync(_dateCursor).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task ChangeAgendaMode(EAgendaMode mode)
    {
        switch (mode)
        {
            case EAgendaMode.Day:
                DateTime date = DateTime.Now.AddDays(1); //normalize
                TimeSpan time = TimeSpan.FromDays(1);
                await _journal.SetCursorsAsync(date, time).ConfigureAwait(false);
                break;
            case EAgendaMode.Week:
                date = DateTime.Now.AddDays(1); //normalize
                time = TimeSpan.FromDays(7);
                await _journal.SetCursorsAsync(date, time).ConfigureAwait(false);
                break;
            case EAgendaMode.Month:
                date = DateTime.Now.AddDays(1); //normalize
                time = TimeSpan.FromDays(30);
                await _journal.SetCursorsAsync(date, time).ConfigureAwait(false);
                break;
            default:
                break;
        }
    }

    [RelayCommand]
    private void CopyToClipboard()
    {
        throw new NotImplementedException();
    }
}