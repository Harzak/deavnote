using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deavnote.repository.Services;

internal sealed class DevTaskRepository : IDevTaskRepository
{
    private readonly IDbContextFactory<DeavnoteDbContext> _contextFactory;

    public DevTaskRepository(IDbContextFactory<DeavnoteDbContext> contextFactory)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);
        _contextFactory = contextFactory;
    }

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
}

