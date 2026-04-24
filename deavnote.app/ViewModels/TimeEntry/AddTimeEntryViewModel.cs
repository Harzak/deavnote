namespace deavnote.app.ViewModels.TimeEntry;

internal sealed partial class AddTimeEntryViewModel : DialogViewModel<AddTimeEntryRequest>
{
    private readonly IDevTaskRepository _taskRepository;

    internal override string Title => Strings.AddTimeEntryViewModel_Title;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor("ConfirmCommand")]
    [Required(ErrorMessage = "Name is required.")]
    public partial string EntryName { get; set; }

    [ObservableProperty]
    public partial string? EntryWorkDone { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor("ConfirmCommand")]
    public partial TimeSpan EntryDuration { get; set; }

    [ObservableProperty]
    public partial DateTimeOffset EntryStartedAt { get; set; }

    [ObservableProperty]
    public partial IEnumerable<DevTaskLightDto> ExistingTasks { get; set; }

    [ObservableProperty]
    public partial DevTaskLightDto? SelectedTask { get; set; }

    [ObservableProperty]
    public partial string SearchTaskCode { get; set; }

    [ObservableProperty]
    public partial string SearchTaskName { get; set; }

    [ObservableProperty]
    public partial ETimeEntryCreationTaskLink EntryTaskLink { get; set; }

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

        this.ExistingTasks = [];
        this.EntryStartedAt = DateTimeOffset.Now;
        this.EntryDuration = TimeSpan.FromHours(1);
        this.SearchTaskCode = string.Empty;
        this.SearchTaskName = string.Empty;
        this.EntryName = string.Empty;
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
