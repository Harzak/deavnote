namespace deavnote.repository.Interfaces;

public interface IDevTaskRepository
{
    Task<IReadOnlyList<DevTaskLightDto>> GetAllLightDtoAsync(CancellationToken cancellationToken = default);
}
