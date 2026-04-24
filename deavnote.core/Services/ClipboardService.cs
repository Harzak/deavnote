namespace deavnote.core.Services;

internal sealed class ClipboardService : IClipboardService
{
    private readonly IClipboardInterop _clipboardInterop;

    public ClipboardService(IClipboardInterop clipboardInterop)
    {
        ArgumentNullException.ThrowIfNull(clipboardInterop);
        _clipboardInterop = clipboardInterop;
    }

    public async Task SetDailyTimeEntryAsync(TimeEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        string text = "";
        await _clipboardInterop.SetTextAsync(text).ConfigureAwait(false);
    }

    public async Task SetDailyTimeEntriesAsync(IEnumerable<TimeEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        string text = "";
        await _clipboardInterop.SetTextAsync(text).ConfigureAwait(false);
    }

    public async Task SetWeeklyTimeEntriesAsync(IEnumerable<TimeEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        string text = "";
        await _clipboardInterop.SetTextAsync(text).ConfigureAwait(false);
    }
}

