using deavnote.app.Enums;
using FluentIcons.Common.Internals;

namespace deavnote.app.ViewModels;

internal sealed partial class AddTimeEntryViewModel : DialogViewModel<TimeEntry>
{
    private readonly IDevTaskRepository _repository;

    internal override string Title => "Add time entry";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor("ConfirmCommand")]
    private string _entryCode = string.Empty;

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
    private IEnumerable<DevTaskLightDto> _availableTasks;

    [ObservableProperty]
    private DevTaskLightDto? _selectedTask;

    [ObservableProperty]
    private string _searchTaskCode;

    [ObservableProperty]
    private string _searchTaskName;

    [ObservableProperty]
    private ETimeEntryCreationTaskLink _entryTaskLink;

    private bool CanConfirm =>
        !string.IsNullOrWhiteSpace(this.EntryCode) &&
        !string.IsNullOrWhiteSpace(this.EntryName) &&
        this.EntryDuration > TimeSpan.Zero;

    public AddTimeEntryViewModel(IDevTaskRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;

        _entryStartedAt = DateTimeOffset.Now;
        _availableTasks = [];
        _searchTaskCode = string.Empty;
        _searchTaskName = string.Empty;
    }

    public async Task InitializedAsync()
    {
        this.AvailableTasks = await _repository.GetAllLightDtoAsync().ConfigureAwait(false);
    }

    [RelayCommand(CanExecute = nameof(CanConfirm))]
    private void Confirm()
    {
        TimeEntry result = new()
        {
            Code = this.EntryCode.Trim(),
            Name = this.EntryName.Trim(),
            WorkDone = string.IsNullOrWhiteSpace(this.EntryWorkDone) ? null : this.EntryWorkDone.Trim(),
            Duration = this.EntryDuration,
            StartedAtUtc = this.EntryStartedAt.UtcDateTime,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };

        base.Close(result);
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
