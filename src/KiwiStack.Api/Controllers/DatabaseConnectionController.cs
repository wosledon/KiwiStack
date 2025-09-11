using KiwiStack.Api.Extensions;
using KiwiStack.Api.Services;
using KiwiStack.Shared.Contracts;
using KiwiStack.Shared.Dtos.DatabaseConnection;
using KiwiStack.Shared.Dtos;
using KiwiStack.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soda.AutoMapper;

namespace KiwiStack.Api.Controllers;

[Authorize]
public class DatabaseConnectionController(
    UnitOfWork db
) : ApiControllerBase
{
    private readonly UnitOfWork _db = db;

    [HttpGet("list")]
    public async Task<IActionResult> GetConnections([FromQuery] SearchDto search)
    {
        var q = _db.Q<DbConnector>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search.Keyword))
        {
            q = q.Where(d => d.Name.Contains(search.Keyword!) || d.Host.Contains(search.Keyword!) || d.Database.Contains(search.Keyword!));
        }

        var paged = await q.OrderByDescending(d => d.CreatedAt).ToPagedListAsync(search.Page, search.PageSize);
        var items = paged.Select(d => d.MapTo<DatabaseConnectionDto>()).ToList();

        return Ok(new KiwiResult<List<DatabaseConnectionDto>> { Data = items, Total = paged.Total });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetConnection(Guid id)
    {
        var conn = await _db.Q<DbConnector>().FirstOrDefaultAsync(c => c.Id == id);
        if (conn == null) return NotFound();
        return Ok(conn.MapTo<DatabaseConnectionDto>());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteConnection(Guid id)
    {
        var conn = await _db.Q<DbConnector>().FirstOrDefaultAsync(c => c.Id == id);
        if (conn == null) return NotFound();

        _db.W.Remove(conn);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteConnections([FromBody] List<Guid> ids)
    {
        var list = await _db.Q<DbConnector>().Where(c => ids.Contains(c.Id)).ToListAsync();
        if (list.Count == 0) return NotFound();
        _db.W.RemoveRange(list);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreateConnection([FromBody] DatabaseConnectionCreateOrUpdateDto dto)
    {
        if (dto == null) return BadRequest();
        var entity = dto.MapTo<DbConnector>();
        // password remains in entity but will not be returned in DTO
        await _db.W.AddAsync(entity);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetConnection), new { id = entity.Id }, entity.MapTo<DatabaseConnectionDto>());
    }

    [HttpPut]
    public async Task<IActionResult> UpdateConnection([FromBody] DatabaseConnectionCreateOrUpdateDto dto)
    {
        if (dto == null || dto.Id == Guid.Empty) return BadRequest();
        var entity = await _db.Q<DbConnector>().FirstOrDefaultAsync(c => c.Id == dto.Id);
        if (entity == null) return NotFound();

        dto.Map(entity);

        _db.W.Update(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
