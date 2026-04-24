using deavnote.model.Entities;
using deavnote.model.Enums;
using Microsoft.EntityFrameworkCore;

namespace deavnote.repository.Services;

/// <summary>
/// Provides data access methods for <see cref="TimeEntry"/> entities
/// </summary>
internal sealed class TimeEntryRepository : ITimeEntryRepository
{
    private readonly IDbContextFactory<DeavnoteDbContext> _contextFactory;
    private readonly ILogger<TimeEntryRepository> _logger;

    public TimeEntryRepository(IDbContextFactory<DeavnoteDbContext> contextFactory, ILogger<TimeEntryRepository> logger)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);
        ArgumentNullException.ThrowIfNull(logger);
        _contextFactory = contextFactory;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TimeEntry>> GetEntriesBetweenAsync(DateOnly startDateUtc, DateOnly endDateUtc, CancellationToken cancellationToken = default)
    {
        if (startDateUtc > endDateUtc)
        {
            throw new ArgumentException("Start date must be less than or equal to end date.");
        }

        DateTime startDateTime = startDateUtc.ToDateTime(TimeOnly.MinValue);
        DateTime endDateTime = endDateUtc.ToDateTime(TimeOnly.MaxValue);

        using DeavnoteDbContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        List<TimeEntry> entries = await context.TimeEntries
          .Where(e => e.StartedAtUtc >= startDateTime && e.StartedAtUtc <= endDateTime)
          .Include(e => e.DevTask)
          .AsNoTracking()
          .ToListAsync(cancellationToken)
          .ConfigureAwait(false);

        return entries.AsReadOnly();
    }

    /// <inheritdoc/>
    public async Task<OperationResult> AddTimeEntryAsync(AddTimeEntryRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        DateTime now = DateTime.UtcNow;

        TimeEntry timeEntry = new()
        {
            Name = request.Name,
            WorkDone = request.WorkDone,
            Duration = request.Duration,
            StartedAtUtc = request.StartedAtUtc,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        using DeavnoteDbContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        switch (request)
        {
            case AddTimeEntryRequest.ForExistingTask existing:
                timeEntry.TaskId = existing.TaskId;
                await context.DevTasks
                    .Where(t => t.Id == existing.TaskId)
                    .ExecuteUpdateAsync(s => s.SetProperty(t => t.UpdatedAtUtc, now), cancellationToken)
                    .ConfigureAwait(false);
                break;

            case AddTimeEntryRequest.ForNewTask newTask:
                timeEntry.DevTask = new DevTask
                {
                    Code = newTask.TaskCode,
                    Name = newTask.TaskName,
                    State = EDevTaskState.InProgress,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now,
                };
                break;
        }

        context.TimeEntries.Add(timeEntry);

        try
        {
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DbUpdateException ex)
        {
            RepositoryLogMessages.LogFailedToAddTimeEntry(_logger, ex);
            return OperationResult.Failure($"Failed to add time entry: {ex.InnerException?.Message}");
        }

        return OperationResult.Success();
    }

    /// <inheritdoc/>
    public async Task<OperationResult> UpdateTimeEntryAsync(UpdateTimeEntryRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using DeavnoteDbContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        TimeEntry? existingEntry = await context.TimeEntries
            .Where(e => e.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (existingEntry == null)
        {
            return OperationResult.Failure($"Time entry with ID {request.Id} not found.");
        }

        existingEntry.Name = request.Name;
        existingEntry.WorkDone = request.WorkDone;
        existingEntry.Duration = request.Duration;
        existingEntry.StartedAtUtc = request.StartedAtUtc;
        existingEntry.UpdatedAtUtc = DateTime.UtcNow;

        try
        {
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DbUpdateException ex)
        {
            RepositoryLogMessages.LogFailedToUpdateTimeEntry(_logger, request.Id, ex);
            return OperationResult.Failure($"Failed to update time entry: {ex.InnerException?.Message}");
        }

        return OperationResult.Success();
    }

    /// <inheritdoc/>
    public async Task<TimeEntry?> GetEntryAsync(int id, CancellationToken cancellationToken = default)
    {
        using DeavnoteDbContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        return await context.TimeEntries
          .Where(e => e.Id == id)
          .Include(e => e.DevTask)
          .AsNoTracking()
          .FirstOrDefaultAsync(cancellationToken)
          .ConfigureAwait(false);
    }
}