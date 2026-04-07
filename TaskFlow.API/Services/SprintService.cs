using TaskFlow.Core.Entities;
using TaskFlow.Core.Interfaces;

namespace TaskFlow.API.Services;

public class SprintService
{
    private readonly ISprintRepository _sprintRepository;

    public SprintService(ISprintRepository sprintRepository)
    {
        _sprintRepository = sprintRepository;
    }

    public async Task<Sprint?> GetSprintByIdAsync(Guid id)
    {
        return await _sprintRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Sprint>> GetAllSprintsAsync()
    {
        return await _sprintRepository.GetAllAsync();
    }

    public async Task CreateSprintAsync(Sprint sprint)
    {
        await _sprintRepository.AddAsync(sprint);
    }

    public async Task UpdateSprintAsync(Sprint sprint)
    {
        await _sprintRepository.UpdateAsync(sprint);
    }

    public async Task DeleteSprintAsync(Guid id)
    {
        await _sprintRepository.DeleteAsync(id);
    }
}