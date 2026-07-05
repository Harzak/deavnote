[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace deavnote.core.Interfaces;

/// <summary>
/// Provides functionality to render templates by replacing placeholders with actual values from TimeEntry objects.
/// </summary>
internal interface ITemplateRenderer
{
    /// <summary>
    /// Renders a template by replacing placeholders with actual values from a <see cref="TimeEntry"/> object.
    /// </summary>
    string RenderTimeEntry(string template, TimeEntry item);

    /// <summary>
    /// Renders a template section for multiple <see cref="TimeEntry"/> items, handling headers, footers, and loops as specified in the template.
    /// </summary>
    string RenderTimeEntries(TemplateSection template, IEnumerable<TimeEntry> items);
}