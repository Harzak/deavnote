using deavnote.repository.Interfaces;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("deavnote.app.tests")]

namespace deavnote.app.ViewModels;

internal sealed partial class JournalViewModel : BaseViewModel
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IJournal _journal;
    private readonly IDateProvider _dateProvider;
    private readonly IDialogService _dialogService;
    private readonly IViewOrchestrator _viewOrchestrator;
    private readonly INotificationService _notificationService;
    private readonly IClipboardService _clipboard;

    [ObservableProperty]
    public partial EJournalContext ViewType { get; set; }

    [ObservableProperty]
    public partial DateOnly DateCursor { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<TimeEntryListItemViewModel> TimeEntries { get; set; }

    [ObservableProperty]
    public partial TimeEntryListItemViewModel? SelectedTimeEntry { get; set; }

    public JournalViewModel(
        IJournal journal,
        IDateProvider dateProvider,
        IViewModelFactory viewModelFactory,
        IDialogService dialogService,
        IViewOrchestrator viewOrchestrator,
        INotificationService notificationService,
        IClipboardService clipboard)
    {
        ArgumentNullException.ThrowIfNull(journal);
        ArgumentNullException.ThrowIfNull(dateProvider);
        ArgumentNullException.ThrowIfNull(viewModelFactory);
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(viewOrchestrator);
        ArgumentNullException.ThrowIfNull(notificationService);
        ArgumentNullException.ThrowIfNull(clipboard);

        _journal = journal;
        _dateProvider = dateProvider;
        _viewModelFactory = viewModelFactory;
        _dialogService = dialogService;
        _viewOrchestrator = viewOrchestrator;
        _notificationService = notificationService;
        _clipboard = clipboard;

        _journal.TimeEntriesChanged += OnJournalTimeEntriesChanged;
        _journal.CursorChanged += OnJournalCursorChanged;
        this.TimeEntries = [];
        this.ViewType = EJournalContext.DailyMultiple;
    }

    public async override Task OnInitializedAsync()
    {
        this.IsLoading = true;
        await _journal.LoadDefaultCursorAsync().ConfigureAwait(false);
        this.IsLoading = false;

        await base.OnInitializedAsync().ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task MoveDateCursorToNowAsync(CancellationToken cancellationToken)
    {
        await _journal.ResetDateCursorAsync(cancellationToken).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task MoveDateCursorToPreviousDayAsync(CancellationToken cancellationToken)
    {
        await _journal.ShiftDateCursorAsync(days: -1, cancellationToken).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task MoveDateCursorToNextDayAsync(CancellationToken cancellationToken)
    {
        await _journal.ShiftDateCursorAsync(days: 1, cancellationToken).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task AddTimeEntryAsync(CancellationToken cancellationToken)
    {
        AddTimeEntryViewModel vm = _viewModelFactory.CreateAddTimeEntryViewModel();
        AddTimeEntryRequest? request = await _dialogService.ShowWindowAsync(vm).ConfigureAwait(false);

        if (request == null)
        {
            return;
        }

        OperationResult result = await _journal.AddEntryAsync(request, cancellationToken).ConfigureAwait(false);

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
    private async Task ChangeJournalMode(EJournalMode mode, CancellationToken cancellationToken)
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
        await _journal.SetCursorsAsync(configuration, cancellationToken).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task CopyToClipboard(CancellationToken cancellationToken)
    {
        switch (this.ViewType)
        {
            case EJournalContext.DailyMultiple:
                await _clipboard.SetDailyTimeEntriesAsync(_journal.TimeEntries, cancellationToken).ConfigureAwait(false);
                break;
            case EJournalContext.Weekly:
                await _clipboard.SetWeeklyTimeEntriesAsync(_journal.TimeEntries, cancellationToken).ConfigureAwait(false);
                break;
            default:
                throw new NotSupportedException(this.ViewType.ToString());
        }
        _notificationService.Show($"{_journal.TimeEntries.Count} time entries copied.", ENotificationType.Success);
    }

    partial void OnSelectedTimeEntryChanged(TimeEntryListItemViewModel? value)
    {
        if (value != null)
        {
            model.Entities.TimeEntry model = _journal.TimeEntries.First(entry => entry.Id == value.Id);
            Task.Run(async () =>
            {
                await _viewOrchestrator.NavigateToTimeEntryDetailAsync(model).ConfigureAwait(false);
            });
        }
    }

    private void OnJournalTimeEntriesChanged(object? sender, TimeEntriesChangedEventArgs e)
    {
        Dispatcher.UIThread.Post((Action)(() =>
        {
            this.TimeEntries.Clear();

            if (_journal.TimeEntries.Count == 0) return;

            foreach (model.Entities.TimeEntry entry in _journal.TimeEntries)
            {
                TimeEntryListItemViewModel timeEntryViewModel = _viewModelFactory.CreateTimeEntryViewModel(entry);
                this.TimeEntries.Add(timeEntryViewModel);
            }
        }));
    }

    private void OnJournalCursorChanged(object? sender, JournalCursorChangedEventArgs e)
    {
        Dispatcher.UIThread.Post((Action)(() =>
        {
            this.DateCursor = e.DateCursor;
        }));
    }
}