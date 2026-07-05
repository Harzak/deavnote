namespace deavnote.core.Services.Template;

/// <summary>
/// Provides functionality to render templates by replacing placeholders with actual values from TimeEntry objects.
/// </summary>
internal sealed partial class TemplateRenderer : ITemplateRenderer
{

    private const string TASK_NAME_PLACEHOLDER = "TaskName";
    private const string TASK_CODE_PLACEHOLDER = "TaskCode";
    private const string ENTRY_NAME_PLACEHOLDER = "EntryName";
    private const string WORK_DONE_PLACEHOLDER = "WorkDone";

    [GeneratedRegex(@"\{(?<Key>\w+)\}", RegexOptions.ExplicitCapture, matchTimeoutMilliseconds: 1000)]
    private static partial Regex PlaceholderReplacementRegex();

    /// <summary>
    /// Renders a template by replacing placeholders with actual values from a <see cref="TimeEntry"/> object.
    /// </summary>
    public string RenderTimeEntry(string template, TimeEntry item)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(item);

        Dictionary<string, string> placeholders = this.CreatePlaceholders(item);

        return PlaceholderReplacementRegex().Replace(template, match =>
        {
            string key = match.Groups[1].Value;
            return placeholders.TryGetValue(key, out string? value) ? value : match.Value;
        });
    }

    /// <summary>
    /// Renders a template section for multiple <see cref="TimeEntry"/> items, handling headers, footers, and loops as specified in the template.
    /// </summary>
    public string RenderTimeEntries(TemplateSection template, IEnumerable<TimeEntry> items)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(items);

        StringBuilder builder = new();

        if (template.HasLoop)
        {
            if (!string.IsNullOrEmpty(template.Header))
            {
                builder.Append(template.Header);
            }

            foreach (TimeEntry item in items)
            {
                string renderedLine = this.RenderTimeEntry(template.Body, item);
                builder.Append(renderedLine);
            }

            if (!string.IsNullOrEmpty(template.Footer))
            {
                builder.Append(template.Footer);
            }
        }
        else
        {
            foreach (TimeEntry item in items)
            {
                string renderedLine = this.RenderTimeEntry(template.Body, item);
                builder.Append(renderedLine);
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// Creates a dictionary of placeholders and their corresponding values from a <see cref="TimeEntry"/> object.
    /// </summary>
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