namespace deavnote.app.Navigation.Guards;

/// <summary>
/// Guard that prevents navigation when there are unsaved changes
/// </summary>
internal sealed class HasUnsavedChangeGuard : INavigationGuard
{
    /// <inheritdoc/>
    public Task<NavigationGuardResult> CanNavigateAsync(IEditableViewModel? from, IEditableViewModel to, NavigationContext context)
    {

        if (from != null && from.HasChanges)
        {
            // todo : show a confirmation dialog to the user before denying navigation
            return Task.FromResult(NavigationGuardResult.Deny("Cannot navigate with unsaved changes"));
        }

        return Task.FromResult(NavigationGuardResult.Allow());
    }
}