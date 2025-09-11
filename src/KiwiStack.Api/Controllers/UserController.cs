using KiwiStack.Api.Services;
using KiwiStack.Api.Extensions;
using System.Linq;
using KiwiStack.Shared.Entities;
using KiwiStack.Shared.Contracts;
using KiwiStack.Shared.Dtos.User;
using KiwiStack.Shared.Dtos;
using KiwiStack.Api.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soda.AutoMapper;

namespace KiwiStack.Api.Controllers;

[Authorize(Roles = "Admin")]
public class UserController(
    UnitOfWork db
) : ApiControllerBase
{
    private readonly UnitOfWork _db = db;

    [HttpGet("list")]
    public async Task<IActionResult> GetUsers([FromQuery] SearchDto dto)
    {
        var q = _db.Q<User>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(dto.Keyword))
        {
            q = q.Where(u => u.Account.Contains(dto.Keyword!) || u.Name.Contains(dto.Keyword!) || u.Email.Contains(dto.Keyword!));
        }

        var paged = await q.OrderByDescending(u => u.CreatedAt)
            .ToPagedListAsync(dto.Page, dto.PageSize);

        var items = paged.Select(u => u.MapTo<UserDto>()).ToList();

        return Ok(new KiwiResult<List<UserDto>> { Data = items, Total = paged.Total });
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _db.Q<User>().FirstOrDefaultAsync(u => u.Id == CurrentUserId);
        if (user == null) return NotFound();
        return Ok(user.MapTo<UserDto>());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _db.Q<User>().FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound();
        return Ok(user.MapTo<UserDto>());
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
    {
        if (dto == null) return BadRequest();

        var (hash, salt) = PasswordHelper.CreateHash(dto.Password);

        var user = dto.MapTo<User>();
        user.PasswordHash = hash;
        user.PasswordSalt = salt;

        await _db.W.AddAsync(user);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user.MapTo<UserDto>());
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto dto)
    {
        if (dto == null || dto.Id == Guid.Empty) return BadRequest();

        var user = await _db.Q<User>().FirstOrDefaultAsync(u => u.Id == dto.Id);
        if (user == null) return NotFound();

        dto.Map(user);

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            var (hash, salt) = PasswordHelper.CreateHash(dto.Password);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
        }

        _db.W.Update(user);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _db.Q<User>().FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound();

        _db.W.Remove(user);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUsers([FromBody] List<Guid> ids)
    {
        if (ids == null || ids.Count == 0)
        {
            return BadRequest("No user IDs provided.");
        }

        var users = await _db.Q<User>()
            .Where(u => ids.Contains(u.Id))
            .ToListAsync();

        if (users.Count == 0)
        {
            return NotFound("No users found for the provided IDs.");
        }

        _db.W.RemoveRange(users);
        await _db.SaveChangesAsync();

        return NoContent();
    }

}
