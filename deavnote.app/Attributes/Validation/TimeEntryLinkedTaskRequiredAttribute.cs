namespace deavnote.app.Attributes.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
internal sealed class TimeEntryLinkedTaskRequiredAttribute : LocalizedValidationAttribute
{
    public TimeEntryLinkedTaskRequiredAttribute()
    {
        base.ErrorMessageResourceName = "AddTimeEntryViewModel_LinkedTask_Required";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        AddTimeEntryViewModel viewModel = (AddTimeEntryViewModel)validationContext.ObjectInstance;

        return viewModel.EntryTaskLink != ETimeEntryCreationTaskLink.LinkToExistingTask || value != null
            ? ValidationResult.Success
            : new ValidationResult(base.GetErrorMessage(validationContext), [validationContext.MemberName ?? string.Empty]);
    }
}