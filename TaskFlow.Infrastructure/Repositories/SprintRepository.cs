using Microsoft.EntityFrameworkCore;
using TaskFlow.Core.Entities;
using TaskFlow.Core.Interfaces;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories;

public class SprintRepository : ISprintRepository
{
    private readonly AppDbContext _context;

    public SprintRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Sprint?> GetByIdAsync(Guid id)
    {
        return await _context.Sprints.FindAsync(id);
    }

    public async Task<IEnumerable<Sprint>> GetAllAsync()
    {
        return await _context.Sprints.ToListAsync();
    }

    public async Task AddAsync(Sprint sprint)
    {
        await _context.Sprints.AddAsync(sprint);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Sprint sprint)
    {
        _context.Sprints.Update(sprint);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var sprint = await GetByIdAsync(id);
        if (sprint != null)
        {
            _context.Sprints.Remove(sprint);
            await _context.SaveChangesAsync();
        }
    }
}