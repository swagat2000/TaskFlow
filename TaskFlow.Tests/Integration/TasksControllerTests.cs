using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using TaskFlow.API;
using TaskFlow.API.DTOs;

namespace TaskFlow.Tests.Integration;

public class TasksControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TasksControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Create_ReturnsCreatedResult_WhenValidData()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = "New Task",
            Description = "Task Description",
            SprintId = Guid.NewGuid(),
            AssignedUserId = Guid.NewGuid()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", taskDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }
}