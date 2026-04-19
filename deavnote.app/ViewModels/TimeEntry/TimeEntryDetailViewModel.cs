namespace deavnote.app.ViewModels.TimeEntry;

internal sealed partial class TimeEntryDetailViewModel : BaseEditableViewModel<(string Name, string WorkDone, TimeSpan Duration)>
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

        this.CommitSnapshot();
    }

    protected override void ApplyChanges()
    {
        throw new NotImplementedException();
    }

    protected override void UndoChanges((string Name, string WorkDone, TimeSpan Duration) snapshot)
    {
        this.Name = snapshot.Name;
        this.WorkDone = snapshot.WorkDone;
        this.Duration = snapshot.Duration;
    }

    protected override (string Name, string WorkDone, TimeSpan Duration) TakeSnapshot()
    {
        return (this.Name, this.WorkDone, this.Duration);
    }

    protected override bool SnapshotEquals((string Name, string WorkDone, TimeSpan Duration) snapshot)
    {
        return snapshot.Name == this.Name
            && snapshot.WorkDone == this.WorkDone
            && snapshot.Duration == this.Duration;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _relatedTask?.Dispose();
        }
        base.Dispose(disposing);
    }
}
