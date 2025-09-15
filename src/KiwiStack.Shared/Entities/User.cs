using KiwiStack.Shared.Contracts;
using KiwiStack.Shared.Enums;

namespace KiwiStack.Shared.Entities;

/// <summary>
/// Represents an application user with authentication and role information.
/// </summary>
public class User : EntityBase
{
    /// <summary>
    /// User account identifier (unique username).
    /// Defaults to an empty string.
    /// </summary>
    public string Account { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the user.
    /// Defaults to an empty string.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the user.
    /// Defaults to an empty string.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hashed password for the user.
    /// Stored as a hash string.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Salt used when hashing the user's password.
    /// </summary>
    public string PasswordSalt { get; set; } = string.Empty;

    /// <summary>
    /// Role assigned to the user (e.g., Admin, User).
    /// Defaults to <see cref="RoleEnum.User"/>.
    /// </summary>
    public RoleEnum Role { get; set; } = RoleEnum.User;

    /// <summary>
    /// Indicates whether the user is an administrator.
    /// </summary>
    public bool IsAdmin => Role == RoleEnum.Admin;
}
