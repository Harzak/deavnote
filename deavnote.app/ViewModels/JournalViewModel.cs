using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;

[assembly: InternalsVisibleTo("deavnote.app.tests")]

namespace deavnote.app.ViewModels;

internal sealed partial class JournalViewModel : BaseViewModel
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IJournal _journal;
    private readonly IDateProvider _dateProvider;
    private readonly IDialogService _dialogService;
    private readonly INotificationService _notificationService;
    private readonly IMessenger _messenger;
    private readonly IClipboardService _clipboard;

    [ObservableProperty]
    private EJournalContext _viewType;

    [ObservableProperty]
    private DateOnly _dateCursor;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private ObservableCollection<TimeEntryListItemViewModel> _timeEntries;

    [ObservableProperty]
    private TimeEntryListItemViewModel? _selectedTimeEntry;

    public JournalViewModel(
        IJournal journal,
        IDateProvider dateProvider,
        IViewModelFactory viewModelFactory,
        IDialogService dialogService,
        INotificationService notificationService,
        IMessenger messenger,
        IClipboardService clipboard)
    {
        ArgumentNullException.ThrowIfNull(journal);
        ArgumentNullException.ThrowIfNull(dateProvider);
        ArgumentNullException.ThrowIfNull(viewModelFactory);
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(notificationService);
        ArgumentNullException.ThrowIfNull(messenger);
        ArgumentNullException.ThrowIfNull(clipboard);

        _journal = journal;
        _dateProvider = dateProvider;
        _viewModelFactory = viewModelFactory;
        _dialogService = dialogService;
        _notificationService = notificationService;
        _messenger = messenger;

        _journal.TimeEntriesChanged += OnJournalTimeEntriesChanged;
        _timeEntries = [];
        _clipboard = clipboard;
    }

    public async Task InitializedAsync()
    {
        this.IsLoading = true;
        await _journal.LoadDefaultCursorAsync().ConfigureAwait(false);
        this.IsLoading = false;
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
    private async Task AddTimeEntryAsync()
    {
        AddTimeEntryViewModel vm = _viewModelFactory.CreateAddTimeEntryViewModel();
        AddTimeEntryRequest? request = await _dialogService.ShowWindowAsync(vm).ConfigureAwait(false);

        if (request == null)
        {
            return;
        }

        OperationResult result = await _journal.AddEntryAsync(request).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            _notificationService.Show($"Time entry added: {Environment.NewLine} [{vm.EntryName}]", ENotificationType.Success);
        }
        else
        {
            _notificationService.Show(result.ErrorMessage ?? "Failed to add time entry.", ENotificationType.Error, durationMs: 0);
        }
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
        switch(this.ViewType)
        {
            case EJournalContext.Daily:
                _clipboard.SetDailyTimeEntriesAsync(_journal.TimeEntries).ConfigureAwait(false);
                break;
            case EJournalContext.Weekly:
                _clipboard.SetWeeklyTimeEntriesAsync(_journal.TimeEntries).ConfigureAwait(false);
                break;
            default:
                throw new NotImplementedException(nameof(this.ViewType));
        }
        _notificationService.Show($"{_journal.TimeEntries.Count} time entries copied.", ENotificationType.Success);
    }

    partial void OnSelectedTimeEntryChanged(TimeEntryListItemViewModel? value)
    {
        if (value != null)
        {
            model.Entities.TimeEntry model = _journal.TimeEntries.First(entry => entry.Id == value.Id);
            _messenger.Send(new TimeEntrySelectedMessage(model));
        }
    }

    private void OnJournalTimeEntriesChanged(object? sender, TimeEntriesChangedEventArgs e)
    {
        Dispatcher.UIThread.Post((Action)(() =>
        {
            this.DateCursor = _journal.DateCursor;

            this.TimeEntries.Clear();

            if (_journal.TimeEntries.Count == 0) return;

            foreach (model.Entities.TimeEntry entry in _journal.TimeEntries)
            {
                TimeEntryListItemViewModel timeEntryViewModel = _viewModelFactory.CreateTimeEntryViewModel(entry);
                this.TimeEntries.Add(timeEntryViewModel);
            }
        }));
    }
}