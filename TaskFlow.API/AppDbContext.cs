using Microsoft.EntityFrameworkCore;
using TaskFlow.Core.Entities;

namespace TaskFlow.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Sprint> Sprints => Set<Sprint>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<Comment> Comments => Set<Comment>();
}