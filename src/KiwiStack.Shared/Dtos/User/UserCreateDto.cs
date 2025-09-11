using System;
using KiwiStack.Shared.Enums;

namespace KiwiStack.Shared.Dtos.User;

public class UserCreateDto
{
    public string Account { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public RoleEnum Role { get; set; } = RoleEnum.User;
}
