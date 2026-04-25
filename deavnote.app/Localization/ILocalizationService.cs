namespace deavnote.app.Localization;

/// <summary>
/// Provides access to localized UI strings and the current UI culture.
/// </summary>
internal interface ILocalizationService : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the culture used to look up localized strings.
    /// Setting this property notifies all bindings so that the UI refreshes.
    /// </summary>
    CultureInfo CurrentCulture { get; set; }

    /// <summary>
    /// Returns the localized string for the supplied resource key, or the key itself
    /// when the resource is not defined.
    /// </summary>
    /// <param name="key">Resource key as defined in the .resx file.</param>
    string GetString(string key);

    /// <summary>
    /// Returns the localized string for the supplied resource key, or the key itself
    /// when the resource is not defined.
    /// </summary>
    /// <param name="key">Resource key as defined in the .resx file.</param>
    string this[string key] { get; }
}
