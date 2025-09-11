using System;

namespace KiwiStack.Shared.Dtos.Auth;

public class LoginDto
{
    public string Account { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}