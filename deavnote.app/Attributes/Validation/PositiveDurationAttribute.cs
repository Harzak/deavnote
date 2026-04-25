namespace deavnote.app.Attributes.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
internal sealed class PositiveDurationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is TimeSpan duration && duration > TimeSpan.Zero)
        {
            return ValidationResult.Success;
        }

        return new ValidationResult("Duration must be greater than zero.", [validationContext.MemberName ?? string.Empty]);
    }
}