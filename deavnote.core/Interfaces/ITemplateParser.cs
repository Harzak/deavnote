[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace deavnote.core.Interfaces;

/// <summary>
/// Parses a template string into distinct sections for single and multiple entries, based on delimiters.
/// </summary>
internal interface ITemplateParser
{
    /// <summary>
    /// Parses the provided template string into a TemplateSection, separating it into header, body, and footer sections based on loop delimiters.
    /// </summary>
    TemplateSection Parse(string template);
}
