namespace deavnote.app.ViewModels;

internal sealed partial class AddTimeEntryViewModel : DialogViewModel<AddTimeEntryRequest>
{
    private readonly IDevTaskRepository _taskRepository;

    internal override string Title => "Add time entry";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor("ConfirmCommand")]
    private string _entryName = string.Empty;

    [ObservableProperty]
    private string? _entryWorkDone;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor("ConfirmCommand")]
    private TimeSpan _entryDuration;

    [ObservableProperty]
    private DateTimeOffset _entryStartedAt;

    [ObservableProperty]
    private IEnumerable<DevTaskLightDto> _existingTasks;

    [ObservableProperty]
    private DevTaskLightDto? _selectedTask;

    [ObservableProperty]
    private string _searchTaskCode;

    [ObservableProperty]
    private string _searchTaskName;

    [ObservableProperty]
    private ETimeEntryCreationTaskLink _entryTaskLink;

    private bool CanConfirm
    {
        get
        {
            bool taskIsValid = this.EntryTaskLink == ETimeEntryCreationTaskLink.LinkToExistingTask
                ? this.SelectedTask != null
                : !string.IsNullOrWhiteSpace(this.SearchTaskCode) && !string.IsNullOrWhiteSpace(this.SearchTaskName);
            return taskIsValid
                && !string.IsNullOrWhiteSpace(this.EntryName)
                && this.EntryDuration > TimeSpan.Zero;
        }
    }

    public AddTimeEntryViewModel(IDevTaskRepository taskRepository)
    {
        ArgumentNullException.ThrowIfNull(taskRepository);
        _taskRepository = taskRepository;

        _existingTasks = [];
        _entryStartedAt = DateTimeOffset.Now;
        _entryDuration = TimeSpan.FromHours(1);
        _searchTaskCode = string.Empty;
        _searchTaskName = string.Empty;
    }

    public async Task InitializedAsync()
    {
        this.ExistingTasks = await _taskRepository.GetAllLightDtoAsync().ConfigureAwait(false);
    }

    [RelayCommand(CanExecute = nameof(CanConfirm))]
    private void Confirm()
    {
        AddTimeEntryRequest request = this.EntryTaskLink switch
        {
            ETimeEntryCreationTaskLink.LinkToExistingTask => new AddTimeEntryRequest.ForExistingTask()
            {
                Name = this.EntryName.Trim(),
                WorkDone = string.IsNullOrWhiteSpace(this.EntryWorkDone) ? null : this.EntryWorkDone.Trim(),
                Duration = this.EntryDuration,
                StartedAtUtc = this.EntryStartedAt.UtcDateTime,
                TaskId = this.SelectedTask!.Id,
            },
            _ => new AddTimeEntryRequest.ForNewTask()
            {
                Name = this.EntryName.Trim(),
                WorkDone = string.IsNullOrWhiteSpace(this.EntryWorkDone) ? null : this.EntryWorkDone.Trim(),
                Duration = this.EntryDuration,
                StartedAtUtc = this.EntryStartedAt.UtcDateTime,
                TaskCode = this.SearchTaskCode,
                TaskName = this.SearchTaskName,
            },
        };

        base.Close(request);
    }

    [RelayCommand]
    private void Cancel() => base.Close(null);

    partial void OnSelectedTaskChanged(DevTaskLightDto? value)
    {
        if (value != null)
        {
            this.EntryTaskLink = ETimeEntryCreationTaskLink.LinkToExistingTask;
        }
    }

    partial void OnSearchTaskCodeChanged(string value)
    {
        if (this.SelectedTask == null)
        {
            this.EntryTaskLink = ETimeEntryCreationTaskLink.CreateNewTask;
        }
    }

    partial void OnSearchTaskNameChanged(string value)
    {
        if (this.SelectedTask == null)
        {
            this.EntryTaskLink = ETimeEntryCreationTaskLink.CreateNewTask;
        }
    }
}
