namespace deavnote.app.ViewModels.TimeEntry;

internal sealed partial class TimeEntryDetailViewModel : BaseViewModel
{
    private readonly model.Entities.TimeEntry _model;

    public DateTimeOffset StartedAtUtc => _model.StartedAtUtc;
    public DateTime CreatedAtUtc => _model.CreatedAtUtc;
    public DateTime UpdatedAtUtc => _model.UpdatedAtUtc;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _workDone;

    [ObservableProperty]
    private TimeSpan _duration;

    [ObservableProperty]
    private DevTaskDetailViewModel _relatedTask;

    public TimeEntryDetailViewModel(model.Entities.TimeEntry model)
    {
        ArgumentNullException.ThrowIfNull(model);
        _model = model;

        _name = model.Name;
        _workDone = model.WorkDone ?? string.Empty;
        _duration = model.Duration;
        _relatedTask = new DevTaskDetailViewModel(model.DevTask);
    }
}
