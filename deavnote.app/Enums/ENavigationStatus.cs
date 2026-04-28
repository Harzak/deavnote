namespace deavnote.app.Enums;

/// <summary>
/// Specifies the possible status of a navigation operation.
/// </summary>
internal enum ENavigationStatus
{
    /// <summary>
    /// Operation is allowed, can naviguate
    /// </summary>
    Allowed,
    /// <summary>
    /// Operation is canceled (i.e. by user action), cannot naviguate
    /// </summary>
    Canceled,
    /// <summary>
    /// Operation is denied (i.e. by error), cannot naviguate
    /// </summary>
    Denied,
}
