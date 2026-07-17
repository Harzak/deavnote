using Avalonia.Markup.Xaml;

namespace deavnote.app.MarkupExtensions;

/// <summary>
/// A markup extension that provides the values of an enum type for use in XAML.
/// </summary>
internal sealed class EnumValuesExtension : MarkupExtension
{
    private readonly Type _enumType;

    public EnumValuesExtension(Type enumType)
    {
        _enumType = enumType;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Enum.GetValues(_enumType);
    }
}