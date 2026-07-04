namespace deavnote.utils.Results;

/// <summary>
/// Represents the result of an operation with success/failure status and error handling capabilities.
/// </summary>
[Serializable]
public class OperationResult : ResultBase
{
    public OperationResult() : base()
    {

    }
    public OperationResult(bool success) : base(success)
    {

    }

    /// <summary>
    /// Indicates whether this result is <see langword="true"/>
    /// enabling short-circuit evaluation with <c>&amp;&amp;</c>.
    /// </summary>
    public bool IsTrue => this.IsSuccess;

    /// <summary>
    /// Sets the operation result to successful status.
    /// </summary>
    /// <returns>The current <see cref="OperationResult"/> instance with the success status set.</returns>
    public OperationResult WithSuccess()
    {
        base.IsSuccess = true;
        return this;
    }

    /// <summary>
    /// Sets the operation result to failed status.
    /// </summary>
    /// <returns>The current <see cref="OperationResult"/> instance with the failure status set.</returns>
    public OperationResult WithFailure()
    {
        base.IsSuccess = false;
        return this;
    }

    /// <summary>
    /// Sets the operation result to failed status with an error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>The current <see cref="OperationResult"/> instance with the failure status and error message set.</returns>
    public OperationResult WithError(string message)
    {
        base.ErrorMessage = message;
        return this.WithFailure();
    }

    /// <summary>
    /// Performs a AND operation with another <see cref="OperationResult"/> instance.
    /// </summary>
    public bool BitwiseAnd(OperationResult other)
    {
        if (this.IsFailed)
        {
            return false;
        }
        return other?.IsSuccess ?? false;
    }


    /// <summary>
    /// Creates a successful operation result.
    /// </summary>
    /// <returns>A new <see cref="OperationResult"/> instance representing a successful operation.</returns>
    public static OperationResult Success()
    {
        return new OperationResult(success: true);
    }

    /// <summary>
    /// Creates a failed operation result.
    /// </summary>
    /// <returns>A new <see cref="OperationResult"/> instance representing a failed operation.</returns>
    public static OperationResult Failure()
    {
        return new OperationResult(success: false);
    }

    /// <summary>
    /// Creates a failed operation result with an error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="OperationResult"/> instance representing a failed operation with the specified error message.</returns>
    public static OperationResult Failure(string message)
    {
        return new OperationResult(success: false).WithError(message);
    }

    /// <summary>
    /// Returns <see langword="true"/> when this result represents a success, enabling short-circuit evaluation with <c>&amp;&amp;</c>.
    /// </summary>
    public static bool operator true(OperationResult result) => result?.IsSuccess ?? false;

    /// <summary>
    /// Returns <see langword="true"/> when this result represents a failure, enabling short-circuit evaluation with <c>&amp;&amp;</c>.
    /// </summary>
    public static bool operator false(OperationResult result) => result?.IsFailed ?? true;

    /// <summary>
    /// Combines two operation results. Returns <paramref name="left"/> when it has failed, otherwise returns <paramref name="right"/>.
    /// </summary>
    public static OperationResult operator &(OperationResult left, OperationResult right)
    {
        if (left == null)
        {
            return Failure();
        }
        if (!left.IsSuccess)
        {
            return left;
        }
        return right;
    }
}
