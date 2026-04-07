using TaskFlow.Core.Enums;

namespace TaskFlow.API.DTOs;

public class TaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public Guid SprintId { get; set; }
    public Guid AssignedUserId { get; set; }
}