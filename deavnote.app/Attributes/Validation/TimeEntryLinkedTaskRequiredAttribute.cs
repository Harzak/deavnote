namespace deavnote.app.Attributes.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
internal sealed class TimeEntryLinkedTaskRequiredAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        AddTimeEntryViewModel viewModel = (AddTimeEntryViewModel)validationContext.ObjectInstance;

        return viewModel.EntryTaskLink != ETimeEntryCreationTaskLink.LinkToExistingTask || value != null
            ? ValidationResult.Success
            : new ValidationResult("Task is required.", [validationContext.MemberName ?? string.Empty]);
    }
}
