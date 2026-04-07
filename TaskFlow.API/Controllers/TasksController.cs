using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.DTOs;
using TaskFlow.Core.Interfaces;

namespace TaskFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskRepository _taskRepository;

    public TasksController(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await _taskRepository.GetAllAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
            return NotFound();

        return Ok(task);
    }

    [HttpPost]
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TaskDto taskDto)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
            return NotFound();

        task.Title = taskDto.Title;
        task.Description = taskDto.Description;
        task.Status = taskDto.Status;
        task.Priority = taskDto.Priority;
        task.SprintId = taskDto.SprintId;
        task.AssignedUserId = taskDto.AssignedUserId;

        await _taskRepository.UpdateAsync(task);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _taskRepository.DeleteAsync(id);
        return NoContent();
    }
}