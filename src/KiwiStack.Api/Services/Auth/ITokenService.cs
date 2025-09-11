using KiwiStack.Shared.Entities;

namespace KiwiStack.Api.Services.Auth;

public interface ITokenService
{
    string GenerateToken(User user);
}
