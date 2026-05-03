namespace deavnote.repository.Interfaces;

public interface ITodoRepository
{
    Task<IReadOnlyList<Todo>> GetAllAsync(ETodoStatus status,  CancellationToken cancellationToken = default);
    Task<OperationResult> AddAsync(Todo item, CancellationToken cancellationToken = default);
    Task<OperationResult> UpdateAsync(Todo item, CancellationToken cancellationToken = default);
}