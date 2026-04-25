namespace deavnote.app.Attributes.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
internal sealed class PositiveDurationAttribute : LocalizedValidationAttribute
{
    public PositiveDurationAttribute()
    {
        base.ErrorMessageResourceName = "AddTimeEntryViewModel_EntryDuration_Positive";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is TimeSpan duration && duration > TimeSpan.Zero)
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(base.GetErrorMessage(validationContext), [validationContext.MemberName ?? string.Empty]);
    }
}