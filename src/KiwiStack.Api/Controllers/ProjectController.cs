using KiwiStack.Api.Extensions;
using KiwiStack.Api.Services;
using KiwiStack.Shared.Contracts;
using KiwiStack.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soda.AutoMapper;
using Microsoft.AspNetCore.Authorization;
using KiwiStack.Shared.Dtos.Project;

namespace KiwiStack.Api.Controllers;

[Authorize]
public class ProjectController(
    UnitOfWork db
) : ApiControllerBase
{
    private readonly UnitOfWork _db = db;

    [HttpGet("list")]
    public async Task<IActionResult> GetProjects(ProjectSearchDto search)
    {
        var projects =
            await _db.Q<Project>()
                .WhereIf(!string.IsNullOrWhiteSpace(search.Keyword), q => q.Where(p => p.Name.Contains(search.Keyword!) || p.Description.Contains(search.Keyword!)))
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Prefix = p.Prefix,
                    CreatedAt = p.CreatedAt
                })
                .ToPagedListAsync(search.Page, search.PageSize);

        return Ok(new KiwiResult<List<ProjectDto>>
        {
            Data = projects,
            Total = projects.Total
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProject(Guid id)
    {
        var project = await _db.Q<Project>()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
        {
            return NotFound();
        }

        return Ok(project);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var project = await _db.Q<Project>()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
        {
            return NotFound();
        }

        _db.W.Remove(project);

        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteProjects([FromBody] List<Guid> ids)
    {
        var projects = await _db.Q<Project>()
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();

        if (projects.Count == 0)
        {
            return NotFound();
        }

        _db.W.RemoveRange(projects);

        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] ProjectCreateOrUpdateDto project)
    {
        if (project == null)
        {
            return BadRequest();
        }

        var entity = project.MapTo<Project>();

        await _db.W.AddAsync(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProject), new { id = entity.Id }, entity);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProject([FromBody] ProjectCreateOrUpdateDto project)
    {
        if (project == null || project.Id == Guid.Empty)
        {
            return BadRequest();
        }

        var entity = await _db.Q<Project>()
            .FirstOrDefaultAsync(p => p.Id == project.Id);

        if (entity == null)
        {
            return NotFound();
        }

        project.Map(entity);


        _db.W.Update(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}