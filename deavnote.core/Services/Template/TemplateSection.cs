namespace deavnote.core.Services.Template;

/// <summary>
/// Represents a parsed template with distinct sections for single and multiple entries.
/// </summary>
internal sealed class TemplateSection : IEquatable<TemplateSection>
{
    public string Header { get; }
    public string Body { get; }
    public string Footer { get; }
    public bool HasLoop { get; }

    public TemplateSection(string header, string body, string footer, bool hasLoop)
    {
        this.Header = header ?? string.Empty;
        this.Body = body ?? string.Empty;
        this.Footer = footer ?? string.Empty;
        this.HasLoop = hasLoop;
    }

    public bool Equals(TemplateSection? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return this.Header.EqualsOrdinalIgnoreCase(other.Header)
            && this.Body.EqualsOrdinalIgnoreCase(other.Body)
            && this.Footer.EqualsOrdinalIgnoreCase(other.Footer)
            && this.HasLoop == other.HasLoop;
    }

    public override bool Equals(object? obj) => obj is TemplateSection other && this.Equals(other);

    public override int GetHashCode() => HashCode.Combine(this.Header, this.Body, this.Footer, this.HasLoop);

    public override string ToString() => this.HasLoop
        ? $"Template[Loop]: Header={this.Header.Length} chars, Body={this.Body.Length} chars, Footer={this.Footer.Length} chars"
        : $"Template[Simple]: {this.Body.Length} chars";
}