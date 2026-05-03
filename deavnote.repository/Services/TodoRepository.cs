namespace deavnote.repository.Services;

internal sealed class TodoRepository : ITodoRepository
{
    private readonly IDbContextFactory<DeavnoteDbContext> _contextFactory;
    private readonly ILogger<TodoRepository> _logger;

    public TodoRepository(IDbContextFactory<DeavnoteDbContext> contextFactory, ILogger<TodoRepository> logger)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);
        ArgumentNullException.ThrowIfNull(logger);
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Todo>> GetAllAsync(ETodoStatus status, CancellationToken cancellationToken = default)
    {
        using DeavnoteDbContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        List<Todo> todos = await context.Todos
            .Where(x => x.Status == status)
            .OrderBy(x => x.CreatedAtUtc)
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return todos.AsReadOnly();
    }

    public async Task<OperationResult> AddAsync(Todo item, CancellationToken cancellationToken = default)
    {
        using DeavnoteDbContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            await context.Todos.AddAsync(item, cancellationToken).ConfigureAwait(false);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DbUpdateException ex)
        {
            TodoLogMessages.LogFailedToAddTodo(_logger, ex);
            return OperationResult.Failure($"Failed to add todo item");
        }

        return OperationResult.Success();
    }

    public async Task<OperationResult> UpdateAsync(Todo item, CancellationToken cancellationToken = default)
    {
        using DeavnoteDbContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        Todo? existingTodo = await context.Todos
            .FirstOrDefaultAsync(x => x.Id == item.Id, cancellationToken)
            .ConfigureAwait(false);

        if (existingTodo == null)
        {
            return OperationResult.Failure($"Todo item with ID {item.Id} not found.");
        }

        existingTodo.Status = item.Status;
        existingTodo.Name = item.Name;
        existingTodo.Description = item.Description;
        existingTodo.Code = item.Code;
        existingTodo.Note = item.Note;
        existingTodo.UpdatedAtUtc = DateTime.UtcNow;

        try
        {
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        }
        catch (DbUpdateException ex)
        {
            TodoLogMessages.LogFailedToUpdateTodo(_logger, item.Id, ex);
            return OperationResult.Failure($"Error updating Todo item");
        }

        return OperationResult.Success();
    }
}

