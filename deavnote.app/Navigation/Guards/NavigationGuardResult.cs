namespace deavnote.app.Navigation.Guards;

/// <summary>
/// Result of navigation guard evaluation
/// </summary>
internal sealed class NavigationGuardResult : OperationResult
{
    public ENavigationStatus Status { get; init; }
    public string? Reason { get; init; }
    public string? RedirectTo { get; init; }

    public bool IsAllowed => Status == ENavigationStatus.Allowed;
    public bool IsDenied => Status == ENavigationStatus.Denied;
    public bool IsCanceled => Status == ENavigationStatus.Canceled;

    public NavigationGuardResult(bool success) : base(success)
    {

    }

    public static NavigationGuardResult Allow()
    {
        return new(success: true)
        {
            Status =  ENavigationStatus.Allowed,
        };
    }

    public static NavigationGuardResult Deny(string reason)
    {
        return new(success: false)
        {
            Status =  ENavigationStatus.Denied,
            Reason = reason,
        };
    }

    public static NavigationGuardResult Cancel(string reason)
    {
        return new(success: false)
        {
            Status = ENavigationStatus.Canceled,
            Reason = reason,
        };
    }
}
