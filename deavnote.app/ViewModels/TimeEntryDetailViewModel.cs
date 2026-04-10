
namespace deavnote.app.ViewModels;

internal sealed partial class TimeEntryDetailViewModel : BaseViewModel
{
    private readonly TimeEntry _model;

    public DateTimeOffset StartedAtUtc => _model.StartedAtUtc;
    public DateTime CreatedAtUtc => _model.CreatedAtUtc;
    public DateTime UpdatedAtUtc => _model.UpdatedAtUtc;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _workDone;

    [ObservableProperty]
    private TimeSpan _duration;

    public TimeEntryDetailViewModel(TimeEntry model)
    {
        ArgumentNullException.ThrowIfNull(model);
        _model = model;

        _name = model.Name;
        _workDone = model.WorkDone ?? string.Empty;
        _duration = model.Duration;
    }
}
