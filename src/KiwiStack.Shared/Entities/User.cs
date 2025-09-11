using KiwiStack.Shared.Contracts;
using KiwiStack.Shared.Enums;

namespace KiwiStack.Shared.Entities;

public class User : EntityBase
{
    public string Account { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;

    public RoleEnum Role { get; set; } = RoleEnum.User;

    public bool IsAdmin => Role == RoleEnum.Admin;
}
