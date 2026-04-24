namespace deavnote.repository.Services;

/// <summary>
/// Provides data access methods for <see cref="DevTask"/> entities
/// </summary>
internal sealed class DevTaskRepository : IDevTaskRepository
{
    private readonly IDbContextFactory<DeavnoteDbContext> _contextFactory;

    public DevTaskRepository(IDbContextFactory<DeavnoteDbContext> contextFactory)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);
        _contextFactory = contextFactory;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<DevTaskLightDto>> GetAllLightDtoAsync(CancellationToken cancellationToken = default)
    {
        using (DeavnoteDbContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
        {
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
    }

    /// <inheritdoc/>
    public async Task<DevTask?> GetTask(int id, CancellationToken cancellationToken = default)
    {
        using (DeavnoteDbContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
        {
            return await context.DevTasks
              .Where(e => e.Id == id)
              .AsNoTracking()
              .FirstOrDefaultAsync(cancellationToken)
              .ConfigureAwait(false);
        }
    }
}

