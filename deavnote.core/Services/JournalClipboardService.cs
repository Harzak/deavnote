using System.Text.RegularExpressions;

namespace deavnote.core.Services;

internal sealed partial class JournalClipboardService : IClipboardService
{
    private readonly IClipboardInterop _clipboardInterop;
    private readonly IClipboardFormatRepository _clipboardFormatRepository;

    private const string TASK_NAME_PLACEHOLDER = "TaskName";
    private const string TASK_CODE_PLACEHOLDER = "TaskCode";
    private const string ENTRY_NAME_PLACEHOLDER = "EntryName";
    private const string WORK_DONE_PLACEHOLDER = "WorkDone";

    public JournalClipboardService(IClipboardInterop clipboardInterop, IClipboardFormatRepository clipboardFormatRepository)
    {
        ArgumentNullException.ThrowIfNull(clipboardInterop);
        ArgumentNullException.ThrowIfNull(clipboardFormatRepository);

        _clipboardInterop = clipboardInterop;
        _clipboardFormatRepository = clipboardFormatRepository;
    }

    public async Task SetDailyTimeEntryAsync(TimeEntry entry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry);

        string text = await this.GetTextAsync(entry, EJournalContext.DailySingle, cancellationToken).ConfigureAwait(false);

        await _clipboardInterop.SetTextAsync(text).ConfigureAwait(false);
    }

    public async Task SetDailyTimeEntriesAsync(IEnumerable<TimeEntry> entries, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entries);

        StringBuilder builder = new();
        foreach (TimeEntry entry in entries)
        {
            string entryText = await this.GetTextAsync(entry, EJournalContext.DailyMultiple, cancellationToken).ConfigureAwait(false);
            builder.AppendLine(entryText);
        }
        await _clipboardInterop.SetTextAsync(builder.ToString()).ConfigureAwait(false);
    }

    public async Task SetWeeklyTimeEntriesAsync(IEnumerable<TimeEntry> entries, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entries);

        StringBuilder builder = new();
        string header = DateOnly.FromDateTime(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture);
        builder.AppendLine(header);
        foreach (TimeEntry entry in entries)
        {
            string entryText = await this.GetTextAsync(entry, EJournalContext.Weekly, cancellationToken).ConfigureAwait(false);
            builder.AppendLine(entryText);
        }

        await _clipboardInterop.SetTextAsync(builder.ToString()).ConfigureAwait(false);
    }

    private async Task<string> GetTextAsync(TimeEntry entry, EJournalContext context, CancellationToken cancellationToken = default)
    {
        string template = await _clipboardFormatRepository.GetTemplateAsync(context, cancellationToken).ConfigureAwait(false);
        Dictionary<string, string> placeholders = this.CreatePlaceholders(entry);
        return this.InterpolateTemplate(template, placeholders);
    }

    [GeneratedRegex(@"\{(\w+)\}", RegexOptions.ExplicitCapture, matchTimeoutMilliseconds: 1000)]
    private static partial Regex PlaceholderReplacementRegex();

    /// <summary>
    /// Replaces placeholders in the template string with corresponding values from the dictionary.
    /// </summary>
    /// <remarks>If a placeholder key is not found in the dictionary, the original placeholder is left unchanged.</remarks>
    private string InterpolateTemplate(string template, Dictionary<string, string> placeholders)
    {
        return PlaceholderReplacementRegex().Replace(template, match =>
        {
            var key = match.Groups[1].Value;
            return placeholders.TryGetValue(key, out var value) ? value : match.Value;
        });
    }

    private Dictionary<string, string> CreatePlaceholders(TimeEntry entry)
    {
        return new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { TASK_NAME_PLACEHOLDER, string.IsNullOrWhiteSpace(entry.DevTask?.Name) ? "[Empty Task Name]" : entry.DevTask.Name },
            { TASK_CODE_PLACEHOLDER, string.IsNullOrWhiteSpace(entry.DevTask?.Code) ? "[Empty Task Code]" : entry.DevTask.Code },
            { ENTRY_NAME_PLACEHOLDER, string.IsNullOrWhiteSpace(entry.Name) ? "[Empty Entry Name]" : entry.Name },
            { WORK_DONE_PLACEHOLDER, entry.WorkDone ?? string.Empty },
        };
    }
}

