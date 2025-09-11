using KiwiStack.Api.Services;
using KiwiStack.Api.Services.Auth;
using KiwiStack.Shared.Dtos;
using KiwiStack.Shared.Dtos.Auth;
using KiwiStack.Shared.Dtos.User;
using KiwiStack.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soda.AutoMapper;

namespace KiwiStack.Api.Controllers;

public class AuthController(
    UnitOfWork db,
    ITokenService tokenService
) : ApiControllerBase
{
    private readonly UnitOfWork _db = db;
    private readonly ITokenService _tokenService = tokenService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (dto == null) return BadRequest();

        var user = await _db.Q<User>()
            .FirstOrDefaultAsync(u => u.Account == dto.Account);

        if (user == null) return Unauthorized();

        if (!PasswordHelper.Verify(dto.Password, user.PasswordHash, user.PasswordSalt))
            return Unauthorized();

        var token = _tokenService.GenerateToken(user);
        return Ok(new { token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserCreateDto dto)
    {
        if (dto == null) return BadRequest();

        var exists = await _db.Q<User>().AnyAsync(u => u.Account == dto.Account || u.Email == dto.Email);
        if (exists) return BadRequest("Account or email already exists.");

        var (hash, salt) = PasswordHelper.CreateHash(dto.Password);

        var user = dto.MapTo<User>();
        user.PasswordHash = hash;
        user.PasswordSalt = salt;

        await _db.W.AddAsync(user);
        await _db.SaveChangesAsync();

        return CreatedAtAction(null, null, user.MapTo<UserDto>());
    }
}
