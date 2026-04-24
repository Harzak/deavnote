namespace deavnote.repository.Services;

/// <summary>
/// Provides data access methods for <see cref="DevTask"/> entities
/// </summary>
internal sealed class DevTaskRepository : IDevTaskRepository
{
    private readonly IDbContextFactory<DeavnoteDbContext> _contextFactory;
    private readonly ILogger<DevTaskRepository> _logger;

    public DevTaskRepository(IDbContextFactory<DeavnoteDbContext> contextFactory, ILogger<DevTaskRepository> logger)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);
        ArgumentNullException.ThrowIfNull(logger);
        _contextFactory = contextFactory;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<DevTaskLightDto>> GetAllLightDtoAsync(CancellationToken cancellationToken = default)
    {
        using DeavnoteDbContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        List<DevTaskLightDto> tasks = await context.DevTasks
            .OrderBy(x => x.CreatedAtUtc)
            .Select(x => new DevTaskLightDto()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return tasks.AsReadOnly();
    }

    /// <inheritdoc/>
    public async Task<DevTask?> GetTaskAsync(int id, CancellationToken cancellationToken = default)
    {
        using DeavnoteDbContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        return await context.DevTasks
          .Where(e => e.Id == id)
          .AsNoTracking()
          .FirstOrDefaultAsync(cancellationToken)
          .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<OperationResult> UpdateTaskAsync(UpdateDevTaskRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using DeavnoteDbContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        DevTask? existingTask = await context.DevTasks
            .Where(e => e.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (existingTask == null)
        {
            return OperationResult.Failure($"Development task with ID {request.Id} not found.");
        }

        existingTask.Code = request.Code;
        existingTask.Name = request.Name;
        existingTask.Description = request.Description;
        existingTask.State = request.State;
        existingTask.UpdatedAtUtc = DateTime.UtcNow;

        try
        {
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DbUpdateException ex)
        {
            RepositoryLogMessages.LogFailedToUpdateDevTask(_logger, request.Id, ex);
            return OperationResult.Failure($"Failed to update development task: {ex.InnerException?.Message}");
        }

        return OperationResult.Success();
    }
}

