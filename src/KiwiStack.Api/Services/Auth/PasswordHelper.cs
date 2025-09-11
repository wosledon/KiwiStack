using System.Security.Cryptography;
using System.Text;

namespace KiwiStack.Api.Services.Auth;

public static class PasswordHelper
{
    public static (string hash, string salt) CreateHash(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(16);
        var salt = Convert.ToBase64String(saltBytes);

        var hash = Hash(password, salt);

        return (hash, salt);
    }

    public static bool Verify(string password, string hash, string salt)
    {
        var newHash = Hash(password, salt);
        return hash == newHash;
    }

    static string Hash(string password, string salt)
    {
        using var hmac = new HMACSHA512(Convert.FromBase64String(salt));
        var bytes = Encoding.UTF8.GetBytes(password);
        var computed = hmac.ComputeHash(bytes);
        return Convert.ToBase64String(computed);
    }
}
