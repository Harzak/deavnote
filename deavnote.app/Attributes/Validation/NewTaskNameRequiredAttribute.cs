namespace deavnote.app.Attributes.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
internal sealed class NewTaskNameRequiredAttribute : ValidationAttribute
{
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

        return new ValidationResult("Task name is required.", [validationContext.MemberName ?? string.Empty]);
    }
}
