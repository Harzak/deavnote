using deavnote.app.Attributes.Validation;

namespace deavnote.app.ViewModels.TimeEntry;

internal sealed partial class AddTimeEntryViewModel : DialogViewModel<AddTimeEntryRequest>
{
    private readonly IDevTaskRepository _taskRepository;

    #region Properties
    internal override string Title => Strings.AddTimeEntryViewModel_Title;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    [Required(ErrorMessage = "Name is required.")]
    public partial string EntryName { get; set; }

    [ObservableProperty]
    public partial string? EntryWorkDone { get; set; }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    [PositiveDuration]
    public partial TimeSpan EntryDuration { get; set; }

    [ObservableProperty]
    public partial DateTimeOffset EntryStartedAt { get; set; }

    [ObservableProperty]
    public partial IEnumerable<DevTaskLightDto> ExistingTasks { get; set; }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    [TimeEntryLinkedTaskRequired]
    public partial DevTaskLightDto? SelectedTask { get; set; }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    [NewDevTaskCodeRequired]
    public partial string SearchTaskCode { get; set; }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    [NewTaskNameRequired]
    public partial string SearchTaskName { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    public partial ETimeEntryCreationTaskLink EntryTaskLink { get; set; }
    #endregion

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

        base.ClearErrors();
    }

    public async Task InitializedAsync()
    {
        this.ExistingTasks = await _taskRepository.GetAllLightDtoAsync().ConfigureAwait(false);
    }

    #region Commands
    [RelayCommand(CanExecute = nameof(CanConfirm))]
    private void Confirm()
    {
        base.ValidateAllProperties();

        if (base.HasErrors)
        {
            return;
        }

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
    #endregion

    #region Property Change Handlers

    partial void OnSelectedTaskChanged(DevTaskLightDto? value)
    {
        if (value != null)
        {
            this.EntryTaskLink = ETimeEntryCreationTaskLink.LinkToExistingTask;
        }
        this.RefreshDevTaskValidation();
    }

    partial void OnSearchTaskCodeChanged(string value)
    {
        if (this.SelectedTask == null)
        {
            this.EntryTaskLink = ETimeEntryCreationTaskLink.CreateNewTask;
        }
        this.RefreshDevTaskValidation();
    }

    partial void OnSearchTaskNameChanged(string value)
    {
        if (this.SelectedTask == null)
        {
            this.EntryTaskLink = ETimeEntryCreationTaskLink.CreateNewTask;
        }
        this.RefreshDevTaskValidation();
    }
    #endregion

    private void RefreshDevTaskValidation()
    {
        base.ValidateProperty(this.SelectedTask, nameof(this.SelectedTask));
        base.ValidateProperty(this.SearchTaskCode, nameof(this.SearchTaskCode));
        base.ValidateProperty(this.SearchTaskName, nameof(this.SearchTaskName));
    }

    private bool CanConfirm()
    {
        List<ValidationResult> results = [];
        ValidationContext validationContext = new(this);

        return Validator.TryValidateObject(
            instance: this,
            validationContext,
            results,
            validateAllProperties: true);
    }
}
