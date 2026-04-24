using Avalonia.Data;
using Avalonia.Markup.Xaml;

namespace deavnote.app.Localization;

/// <summary>
/// XAML markup extension that resolves a localized string from the
/// <see cref="LocalizationService"/> singleton.
/// </summary>
/// <example>
/// <code>
/// &lt;TextBlock Text="{loc:Localize Key=MainView_Title}" /&gt;
/// </code>
/// </example>
internal sealed class LocalizeExtension : MarkupExtension
{
    public LocalizeExtension()
    {
    }

    public LocalizeExtension(string key)
    {
        this.Key = key;
    }

    /// <summary>
    /// Resource key as defined in the .resx file.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        // Bind to the indexer of the singleton localization service so that
        // a culture change automatically refreshes the displayed text.
        return new Binding($"[{this.Key}]")
        {
            Mode = BindingMode.OneWay,
            Source = LocalizationService.Instance,
        };
    }
}
