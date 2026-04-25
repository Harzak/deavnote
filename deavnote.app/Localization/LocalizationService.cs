using System.Resources;

namespace deavnote.app.Localization;

/// <summary>
/// Default <see cref="ILocalizationService"/> implementation backed by the
/// <see cref="Strings"/> resource manager.
/// </summary>
/// <remarks>
/// Exposes a static <see cref="Instance"/> so that the <see cref="LocalizeExtension"/>
/// XAML markup extension can resolve the singleton without DI access from XAML.
/// </remarks>
internal sealed class LocalizationService : ILocalizationService
{
    // Indexer property name used by WPF/Avalonia bindings to refresh "this[...]" bindings.
    private const string IndexerPropertyName = "Item[]";

    private readonly ResourceManager _resourceManager;
    private CultureInfo _currentCulture;

    public LocalizationService()
        : this(Strings.ResourceManager, CultureInfo.CurrentUICulture)
    {
    }

    public LocalizationService(ResourceManager resourceManager, CultureInfo currentCulture)
    {
        ArgumentNullException.ThrowIfNull(resourceManager);
        ArgumentNullException.ThrowIfNull(currentCulture);

        _resourceManager = resourceManager;
        _currentCulture = currentCulture;
        Strings.Culture = currentCulture;
    }

    /// <summary>
    /// Singleton instance used by the <see cref="LocalizeExtension"/> markup extension.
    /// Initialized from <see cref="App"/> startup.
    /// </summary>
    public static LocalizationService Instance { get; } = new LocalizationService();

    public event PropertyChangedEventHandler? PropertyChanged;

    public CultureInfo CurrentCulture
    {
        get => _currentCulture;
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            bool changed = !_currentCulture.Equals(value);

            _currentCulture = value;
            Strings.Culture = value;
            CultureInfo.CurrentUICulture = value;
            CultureInfo.DefaultThreadCurrentUICulture = value;

            if (!changed)
            {
                return;
            }

            // Notify both the named property and the indexer so all
            // {loc:Localize ...} bindings refresh.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCulture)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerPropertyName));
        }
    }

    public string GetString(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return string.Empty;
        }

        return _resourceManager.GetString(key, _currentCulture) ?? key;
    }

    public string this[string key] => this.GetString(key);
}
