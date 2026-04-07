using Moq;
using TaskFlow.API.Services;
using TaskFlow.Core.Entities;
using TaskFlow.Core.Interfaces;

namespace TaskFlow.Tests.Unit;

public class SprintServiceTests
{
    private readonly Mock<ISprintRepository> _sprintRepositoryMock;
    private readonly SprintService _sprintService;

    public SprintServiceTests()
    {
        _sprintRepositoryMock = new Mock<ISprintRepository>();
        _sprintService = new SprintService(_sprintRepositoryMock.Object);
    }

    [Fact]
    public async Task GetSprintByIdAsync_ReturnsSprint_WhenSprintExists()
    {
        // Arrange
        var sprintId = Guid.NewGuid();
        var expectedSprint = new Sprint { Id = sprintId, Name = "Test Sprint" };
        _sprintRepositoryMock.Setup(repo => repo.GetByIdAsync(sprintId)).ReturnsAsync(expectedSprint);

        // Act
        var result = await _sprintService.GetSprintByIdAsync(sprintId);

        // Assert
        Assert.Equal(expectedSprint, result);
    }

    [Fact]
    public async Task GetSprintByIdAsync_ReturnsNull_WhenSprintDoesNotExist()
    {
        // Arrange
        var sprintId = Guid.NewGuid();
        _sprintRepositoryMock.Setup(repo => repo.GetByIdAsync(sprintId)).ReturnsAsync((Sprint?)null);

        // Act
        var result = await _sprintService.GetSprintByIdAsync(sprintId);

        // Assert
        Assert.Null(result);
    }
}