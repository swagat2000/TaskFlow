using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.DTOs;
using TaskFlow.Core.Interfaces;

namespace TaskFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SprintsController : ControllerBase
{
    private readonly ISprintRepository _sprintRepository;

    public SprintsController(ISprintRepository sprintRepository)
    {
        _sprintRepository = sprintRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sprints = await _sprintRepository.GetAllAsync();
        return Ok(sprints);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var sprint = await _sprintRepository.GetByIdAsync(id);
        if (sprint == null)
            return NotFound();

        return Ok(sprint);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SprintDto sprintDto)
    {
        var sprint = new TaskFlow.Core.Entities.Sprint
        {
            Name = sprintDto.Name,
            StartDate = sprintDto.StartDate,
            EndDate = sprintDto.EndDate,
            ProjectId = sprintDto.ProjectId
        };

        await _sprintRepository.AddAsync(sprint);
        return CreatedAtAction(nameof(GetById), new { id = sprint.Id }, sprint);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SprintDto sprintDto)
    {
        var sprint = await _sprintRepository.GetByIdAsync(id);
        if (sprint == null)
            return NotFound();

        sprint.Name = sprintDto.Name;
        sprint.StartDate = sprintDto.StartDate;
        sprint.EndDate = sprintDto.EndDate;
        sprint.ProjectId = sprintDto.ProjectId;

        await _sprintRepository.UpdateAsync(sprint);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sprintRepository.DeleteAsync(id);
        return NoContent();
    }
}