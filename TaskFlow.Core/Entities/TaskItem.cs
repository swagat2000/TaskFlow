using TaskFlow.Core.Enums;
using TaskStatus = TaskFlow.Core.Enums.TaskStatus;

namespace TaskFlow.Core.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public Guid SprintId { get; set; }
    public Sprint Sprint { get; set; } = null!;
    public Guid? AssignedUserId { get; set; }
    public User? AssignedUser { get; set; }
    public List<Comment> Comments { get; set; } = [];
}