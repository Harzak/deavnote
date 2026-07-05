namespace deavnote.core.Services.Template;

/// <summary>
/// Parses a template string into distinct sections for single and multiple entries, based on delimiters.
/// </summary>
internal sealed class TemplateParser : ITemplateParser
{
    private const string LOOP_START_DELIMITER = "{{EACH_ENTRY}}";
    private const string LOOP_END_DELIMITER = "{{END_EACH}}";

    /// <summary>
    /// Parses the provided template string into a TemplateSection, separating it into header, body, and footer sections based on loop delimiters.
    /// </summary>
    public TemplateSection Parse(string template)
    {
        ArgumentNullException.ThrowIfNull(template);

        int loopStartIndex = template.IndexOf(LOOP_START_DELIMITER, StringComparison.Ordinal);
        int loopEndIndex = template.IndexOf(LOOP_END_DELIMITER, StringComparison.Ordinal);

        if (loopStartIndex == -1 || loopEndIndex == -1)
        {
            return new TemplateSection(string.Empty, template, string.Empty, hasLoop: false);
        }

        if (loopEndIndex < loopStartIndex)
        {
            throw new InvalidOperationException("Template error: {{END_EACH}} appears before {{EACH_ENTRY}}");
        }

        int headerEnd = template.FindLineStart(loopStartIndex);
        string header = template.Substring(0, headerEnd);

        int bodyStart = template.FindLineEnd(loopStartIndex);

        int bodyEnd = template.FindLineStart(loopEndIndex);
        string body = template.Substring(bodyStart, bodyEnd - bodyStart);

        int footerStart = template.FindLineEnd(loopEndIndex);
        string footer = template.Substring(footerStart);

        return new TemplateSection(header, body, footer, hasLoop: true);
    }
}