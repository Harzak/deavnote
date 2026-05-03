namespace deavnote.app.Navigation.States;

/// <summary>
/// Default navigation state for view models without navigation-specific state.
/// </summary>
internal sealed class EmptyNavigationStateDescriptor : INavigationStateDescriptor
{
    public static EmptyNavigationStateDescriptor Instance { get; } = new();

    public bool HasUnsavedChanges => false;

    private EmptyNavigationStateDescriptor()
    {
    }

    public Task<OperationResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(OperationResult.Success());
    }
}
