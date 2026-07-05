namespace deavnote.core.Services;

/// <summary>
/// Service responsible for formatting time entries and setting them to the clipboard -
/// based on user-defined templates for different journal modes (single entry, daily, weekly).
/// </summary>
internal sealed class JournalClipboardService : IClipboardService
{
    private readonly IClipboardInterop _clipboardInterop;
    private readonly IClipboardFormatRepository _clipboardFormatRepository;
    private readonly ITemplateParser _templateParser;
    private readonly ITemplateRenderer _templateRenderer;

    public JournalClipboardService(
          IClipboardInterop clipboardInterop,
          IClipboardFormatRepository clipboardFormatRepository,
          ITemplateParser templateParser,
          ITemplateRenderer templateRenderer)
    {
        ArgumentNullException.ThrowIfNull(clipboardInterop);
        ArgumentNullException.ThrowIfNull(clipboardFormatRepository);
        ArgumentNullException.ThrowIfNull(templateParser);
        ArgumentNullException.ThrowIfNull(templateRenderer);

        _clipboardInterop = clipboardInterop;
        _clipboardFormatRepository = clipboardFormatRepository;
        _templateParser = templateParser;
        _templateRenderer = templateRenderer;
    }

    /// <summary>
    /// Sets the clipboard text to a formatted representation of a single time entry.
    /// </summary>
    public async Task SetTimeEntryAsync(TimeEntry entry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry);

        string templateString = await _clipboardFormatRepository.GetTemplateAsync(EJournalMode.TimeEntry, cancellationToken).ConfigureAwait(false);
        string text = _templateRenderer.RenderTimeEntry(templateString, entry);

        await _clipboardInterop.SetTextAsync(text).ConfigureAwait(false);
    }

    /// <summary>
    /// Sets the clipboard text to a formatted representation of multiple time entries for a single day.
    /// </summary>
    public async Task SetDailyTimeEntriesAsync(IEnumerable<TimeEntry> entries, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entries);

        string templateString = await _clipboardFormatRepository.GetTemplateAsync(EJournalMode.Day, cancellationToken).ConfigureAwait(false);
        TemplateSection template = _templateParser.Parse(templateString);
        string text = _templateRenderer.RenderTimeEntries(template, entries);

        await _clipboardInterop.SetTextAsync(text).ConfigureAwait(false);
    }

    /// <summary>
    /// Sets the clipboard text to a formatted representation of multiple time entries for a week.
    /// </summary>
    public async Task SetWeeklyTimeEntriesAsync(IEnumerable<TimeEntry> entries, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entries);

        string templateString = await _clipboardFormatRepository.GetTemplateAsync(EJournalMode.Week, cancellationToken).ConfigureAwait(false);
        TemplateSection template = _templateParser.Parse(templateString);

        StringBuilder builder = new();
        string header = DateOnly.FromDateTime(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture);
        builder.AppendLine(header);

        string text = _templateRenderer.RenderTimeEntries(template, entries);
        builder.Append(text);

        await _clipboardInterop.SetTextAsync(builder.ToString()).ConfigureAwait(false);
    }
}
