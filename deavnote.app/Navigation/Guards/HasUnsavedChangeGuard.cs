namespace deavnote.app.Navigation.Guards;

/// <summary>
/// Guard that prevents navigation when there are unsaved changes
/// </summary>
internal sealed class HasUnsavedChangeGuard : INavigationGuard
{
    private readonly IDialogService _dialogService;

    public HasUnsavedChangeGuard(IDialogService dialogService)
    {
        ArgumentNullException.ThrowIfNull(dialogService);

        _dialogService = dialogService;
    }

    /// <inheritdoc/>
    public async Task<NavigationGuardResult> CanNavigateAsync(IEditableViewModel? from, IEditableViewModel to, NavigationContext context)
    {
        if (from?.HasChanges == true)
        {
            ConfirmationViewModel vm = new(Strings.AskUnsavedChanges);
            EConfirmationResult? result = await _dialogService.ShowWindowAsync(vm).ConfigureAwait(false);

            switch (result)
            {
                case EConfirmationResult.Yes:
                    OperationResult saveResult = await from.TrySaveAsync().ConfigureAwait(false);
                    if (saveResult.IsSuccess)
                    {
                        return NavigationGuardResult.Allow();
                    }
                    return NavigationGuardResult.Deny(saveResult.ErrorMessage);

                case EConfirmationResult.No:
                    return NavigationGuardResult.Allow();
                case EConfirmationResult.Cancel:
                case null:
                    return NavigationGuardResult.Cancel(Strings.UserCancelledGuard);
                default:
                    throw new NotSupportedException(result.ToString());
            }

        }

        return NavigationGuardResult.Allow();
    }
}