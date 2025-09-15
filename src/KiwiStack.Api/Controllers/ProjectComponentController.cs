using KiwiStack.Api.Extensions;
using KiwiStack.Api.Services;
using KiwiStack.Shared.Contracts;
using KiwiStack.Shared.Dtos.ProjectComponent;
using KiwiStack.Shared.Dtos;
using KiwiStack.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soda.AutoMapper;

namespace KiwiStack.Api.Controllers;

[Authorize]
public class ProjectComponentController(
    UnitOfWork db
) : ApiControllerBase
{
    private readonly UnitOfWork _db = db;

    [HttpGet("list")]
    public async Task<IActionResult> GetComponents([FromQuery] SearchDto search)
    {
        var q = _db.Q<SubProject>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search.Keyword))
        {
            q = q.Where(c => c.Name.Contains(search.Keyword!));
        }

        var paged =
            await q.OrderByDescending(c => c.CreatedAt)
                .Select(c=> new ProjectComponentDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ProjectId = c.ProjectId,
                    CreatedAt = c.CreatedAt
                })
                .ToPagedListAsync(search.Page, search.PageSize);

        return Ok(new KiwiResult<List<ProjectComponentDto>> { Data = paged, Total = paged.Total });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetComponent(Guid id)
    {
        var comp = await _db.Q<SubProject>().FirstOrDefaultAsync(c => c.Id == id);
        if (comp == null) return NotFound();
        return Ok(comp.MapTo<ProjectComponentDto>());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteComponent(Guid id)
    {
        var comp = await _db.Q<SubProject>().FirstOrDefaultAsync(c => c.Id == id);
        if (comp == null) return NotFound();

        _db.W.Remove(comp);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteComponents([FromBody] List<Guid> ids)
    {
        var list = await _db.Q<SubProject>().Where(c => ids.Contains(c.Id)).ToListAsync();
        if (list.Count == 0) return NotFound();
        _db.W.RemoveRange(list);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreateComponent([FromBody] ProjectComponentCreateOrUpdateDto dto)
    {
        if (dto == null) return BadRequest();
        var entity = dto.MapTo<SubProject>();
        await _db.W.AddAsync(entity);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetComponent), new { id = entity.Id }, entity.MapTo<ProjectComponentDto>());
    }

    [HttpPut]
    public async Task<IActionResult> UpdateComponent([FromBody] ProjectComponentCreateOrUpdateDto dto)
    {
        if (dto == null || dto.Id == Guid.Empty) return BadRequest();
        var entity = await _db.Q<SubProject>().FirstOrDefaultAsync(c => c.Id == dto.Id);
        if (entity == null) return NotFound();

        dto.Map(entity);

        _db.W.Update(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
