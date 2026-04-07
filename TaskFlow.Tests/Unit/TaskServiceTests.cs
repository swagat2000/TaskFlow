using Moq;
using TaskFlow.API.Services;
using TaskFlow.Core.Entities;
using TaskFlow.Core.Interfaces;

namespace TaskFlow.Tests.Unit;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _taskService = new TaskService(_taskRepositoryMock.Object);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ReturnsTask_WhenTaskExists()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var expectedTask = new TaskItem { Id = taskId, Title = "Test Task" };
        _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync(expectedTask);

        // Act
        var result = await _taskService.GetTaskByIdAsync(taskId);

        // Assert
        Assert.Equal(expectedTask, result);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ReturnsNull_WhenTaskDoesNotExist()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _taskService.GetTaskByIdAsync(taskId);

        // Assert
        Assert.Null(result);
    }
}