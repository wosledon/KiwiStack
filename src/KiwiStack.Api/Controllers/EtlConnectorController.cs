using KiwiStack.Api.Extensions;
using KiwiStack.Api.Services;
using KiwiStack.Shared.Contracts;
using KiwiStack.Shared.Dtos.EtlConnector;
using KiwiStack.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soda.AutoMapper;
using KiwiStack.Shared.Dtos;

namespace KiwiStack.Api.Controllers;

[Authorize]
public class EtlConnectorController(
    UnitOfWork db
) : ApiControllerBase
{
    private readonly UnitOfWork _db = db;

    [HttpGet("list")]
    public async Task<IActionResult> GetConnectors([FromQuery] SearchDto search)
    {
        var q = _db.Q<EtlConnector>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search.Keyword))
        {
            q = q.Where(e => e.Name.Contains(search.Keyword!) || e.Description.Contains(search.Keyword!));
        }

        var paged = await q.OrderByDescending(e => e.CreatedAt).ToPagedListAsync(search.Page, search.PageSize);
        var items = paged.Select(e => e.MapTo<EtlConnectorDto>()).ToList();

        return Ok(new KiwiResult<List<EtlConnectorDto>> { Data = items, Total = paged.Total });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetConnector(Guid id)
    {
        var item = await _db.Q<EtlConnector>().FirstOrDefaultAsync(e => e.Id == id);
        if (item == null) return NotFound();
        return Ok(item.MapTo<EtlConnectorDto>());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteConnector(Guid id)
    {
        var item = await _db.Q<EtlConnector>().FirstOrDefaultAsync(e => e.Id == id);
        if (item == null) return NotFound();
        _db.W.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteConnectors([FromBody] List<Guid> ids)
    {
        var list = await _db.Q<EtlConnector>().Where(e => ids.Contains(e.Id)).ToListAsync();
        if (list.Count == 0) return NotFound();
        _db.W.RemoveRange(list);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreateConnector([FromBody] EtlConnectorCreateOrUpdateDto dto)
    {
        if (dto == null) return BadRequest();
        var entity = dto.MapTo<EtlConnector>();
        await _db.W.AddAsync(entity);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetConnector), new { id = entity.Id }, entity.MapTo<EtlConnectorDto>());
    }

    [HttpPut]
    public async Task<IActionResult> UpdateConnector([FromBody] EtlConnectorCreateOrUpdateDto dto)
    {
        if (dto == null || dto.Id == Guid.Empty) return BadRequest();
        var entity = await _db.Q<EtlConnector>().FirstOrDefaultAsync(e => e.Id == dto.Id);
        if (entity == null) return NotFound();

        dto.Map(entity);
        _db.W.Update(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
