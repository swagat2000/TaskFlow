namespace TaskFlow.Core.Entities;

public class Sprint
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public List<TaskItem> Tasks { get; set; } = [];
}