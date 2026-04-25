namespace deavnote.app.Attributes.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
internal sealed class NewDevTaskNameRequiredAttribute : LocalizedValidationAttribute
{
    public NewDevTaskNameRequiredAttribute()
    {
        base.ErrorMessageResourceName = "AddTimeEntryViewModel_NewTaskName_Required";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string name)
        {
            AddTimeEntryViewModel viewModel = (AddTimeEntryViewModel)validationContext.ObjectInstance;

            if (viewModel.EntryTaskLink != ETimeEntryCreationTaskLink.CreateNewTask || !string.IsNullOrWhiteSpace(name))
            {
                return ValidationResult.Success;
            }
        }

        return new ValidationResult(base.GetErrorMessage(validationContext), [validationContext.MemberName ?? string.Empty]);
    }
}
