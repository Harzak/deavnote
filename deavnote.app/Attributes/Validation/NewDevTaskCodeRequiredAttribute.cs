namespace deavnote.app.Attributes.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
internal sealed class NewDevTaskCodeRequiredAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string code)
        {
            AddTimeEntryViewModel viewModel = (AddTimeEntryViewModel)validationContext.ObjectInstance;

            if( viewModel.EntryTaskLink != ETimeEntryCreationTaskLink.CreateNewTask || !string.IsNullOrWhiteSpace(code))
            {
                return ValidationResult.Success;
            }
        }

        return new ValidationResult("Task code is required.", [validationContext.MemberName ?? string.Empty]);
    }
}
