using TaskFlow.Core.Entities;

namespace TaskFlow.Core.Interfaces;

public interface ISprintRepository
{
    Task<Sprint?> GetByIdAsync(Guid id);
    Task<IEnumerable<Sprint>> GetAllAsync();
    Task AddAsync(Sprint sprint);
    Task UpdateAsync(Sprint sprint);
    Task DeleteAsync(Guid id);
}