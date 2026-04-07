namespace TaskFlow.Core.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; } = null!;
}