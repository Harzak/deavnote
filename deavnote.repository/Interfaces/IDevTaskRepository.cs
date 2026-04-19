namespace deavnote.repository.Interfaces;

/// <summary>
/// Provides data access methods for <see cref="DevTask"/> entities
/// </summary>
public interface IDevTaskRepository
{
    /// <summary>
    /// Asynchronously retrieves a read-only list of lightweight development task DTOs ordered by creation date.
    /// </summary>
    Task<IReadOnlyList<DevTaskLightDto>> GetAllLightDtoAsync(CancellationToken cancellationToken = default);


    /// <summary>
    /// Retrieves a DevTask entity by its identifier.
    /// </summary>
    Task<DevTask?> GetTaskAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates an existing development task.
    /// </summary>
    Task<OperationResult> UpdateTaskAsync(UpdateDevTaskRequest request, CancellationToken cancellationToken = default);
}
