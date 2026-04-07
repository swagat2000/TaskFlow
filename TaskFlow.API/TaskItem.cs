namespace TaskFlow.Core.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid SprintId { get; set; }
    public Sprint Sprint { get; set; } = null!;
    public List<Comment> Comments { get; set; } = [];
}