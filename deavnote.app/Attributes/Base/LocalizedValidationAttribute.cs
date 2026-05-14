
namespace deavnote.app.Attributes.Base;

internal abstract class LocalizedValidationAttribute : ValidationAttribute
{
    private readonly string _errorMessageResourceName;

    protected LocalizedValidationAttribute(string errorMessageResourceName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessageResourceName);

        _errorMessageResourceName = errorMessageResourceName;
    }

    protected string GetErrorMessage(ValidationContext validationContext)
    {
        if (validationContext.ObjectInstance is ILocalizedValidationContext localizedValidationContext)
        {
            return localizedValidationContext.LocalizationService.GetString(_errorMessageResourceName);
        }

        return _errorMessageResourceName;
    }
}
