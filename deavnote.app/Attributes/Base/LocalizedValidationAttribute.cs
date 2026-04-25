
namespace deavnote.app.Attributes.Base;

internal abstract class LocalizedValidationAttribute : ValidationAttribute
{
    protected string GetErrorMessage(ValidationContext validationContext)
    {
        if (validationContext.ObjectInstance is ILocalizedValidationContext localizedValidationContext)
        {
            return localizedValidationContext.LocalizationService.GetString(base.ErrorMessageResourceName ?? string.Empty);
        }

        return base.ErrorMessageResourceName ?? string.Empty;
    }
}
