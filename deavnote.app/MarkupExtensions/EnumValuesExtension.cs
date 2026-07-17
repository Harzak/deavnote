using Avalonia.Markup.Xaml;

namespace deavnote.app.MarkupExtensions;

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