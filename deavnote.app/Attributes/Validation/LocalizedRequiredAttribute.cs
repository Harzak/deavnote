using deavnote.app.Attributes.Base;

namespace deavnote.app.Attributes.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
internal sealed class LocalizedRequiredAttribute : RequiredAttribute
{
    public LocalizedRequiredAttribute(string errorMessageResourceName)
    {
        base.ErrorMessageResourceName = errorMessageResourceName;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        return base.IsValid(value)
            ? ValidationResult.Success
            : new ValidationResult(this.GetErrorMessage(validationContext), [validationContext.MemberName ?? string.Empty]);
    }

    private string GetErrorMessage(ValidationContext validationContext)
    {
        if (validationContext.ObjectInstance is ILocalizedValidationContext localizedValidationContext)
        {
            return localizedValidationContext.LocalizationService.GetString(base.ErrorMessageResourceName ?? string.Empty);
        }

        return base.ErrorMessageResourceName ?? string.Empty;
    }
}
