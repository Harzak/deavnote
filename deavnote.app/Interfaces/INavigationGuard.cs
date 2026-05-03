namespace deavnote.app.Interfaces;

/// <summary>
/// Interface for navigation guards that control navigation flow
/// </summary>
internal interface INavigationGuard
{
    /// <summary>
    /// Determines whether navigation from the specified source view model to the target view model is allowed.
    /// </summary>
    Task<NavigationGuardResult> CanNavigateAsync(IViewModel? from, IViewModel to, NavigationContext context);
}
