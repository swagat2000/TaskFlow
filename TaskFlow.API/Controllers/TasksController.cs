using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.API.DTOs;
using TaskFlow.Core.Interfaces;

namespace TaskFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskRepository _taskRepository;

    public TasksController(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    /// <summary>
    /// Get all tasks - accessible by all authenticated users (Developers, Managers, Admins)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "DeveloperAndAbove")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await _taskRepository.GetAllAsync();
        return Ok(tasks);
    }

    /// <summary>
    /// Get task by ID - accessible by all authenticated users
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "DeveloperAndAbove")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
            return NotFound();

        return Ok(task);
    }

    /// <summary>
    /// Create task - only Manager and Admin can create
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] TaskDto taskDto)
    {
        var task = new TaskFlow.Core.Entities.TaskItem
        {
            Title = taskDto.Title,
            Description = taskDto.Description,
            Status = taskDto.Status,
            Priority = taskDto.Priority,
            SprintId = taskDto.SprintId,
            AssignedUserId = taskDto.AssignedUserId
        };

        await _taskRepository.AddAsync(task);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    /// <summary>
    /// Update task - Manager/Admin can update any task, Developer can update own tasks
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "DeveloperAndAbove")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(Guid id, [FromBody] TaskDto taskDto)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
            return NotFound();

        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Developers can only update their own tasks
        if (userRole == "Developer" && task.AssignedUserId.ToString() != userId)
            return Forbid("You can only update your own tasks");

        task.Title = taskDto.Title;
        task.Description = taskDto.Description;
        task.Status = taskDto.Status;
        task.Priority = taskDto.Priority;
        task.SprintId = taskDto.SprintId;
        task.AssignedUserId = taskDto.AssignedUserId;

        await _taskRepository.UpdateAsync(task);
        return NoContent();
    }

    /// <summary>
    /// Delete task - only Manager and Admin can delete
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _taskRepository.DeleteAsync(id);
        return NoContent();
    }
}