using System;
using KiwiStack.Shared.Enums;

namespace KiwiStack.Shared.Dtos.User;

public class UserUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public RoleEnum Role { get; set; } = RoleEnum.User;
}
