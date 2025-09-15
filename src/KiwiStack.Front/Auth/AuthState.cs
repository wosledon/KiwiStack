using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace KiwiStack.Front.Auth;

public class ApiAuthStateProvider(ITokenStorage storage) : AuthenticationStateProvider, ITokenProvider
{
    private readonly ITokenStorage _storage = storage;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _storage.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        var identity = JwtParser.ToClaimsIdentity(token);
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyAuthenticationStateChanged() => base.NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    Task<string?> ITokenProvider.GetTokenAsync() => _storage.GetTokenAsync();
}

internal static class JwtParser
{
    public static ClaimsIdentity ToClaimsIdentity(string jwt)
    {
        try
        {
            var payload = jwt.Split('.')[1];
            payload = PadBase64(payload);
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new();

            var claims = new List<Claim>();
            foreach (var kv in dict)
            {
                if (kv.Value is System.Text.Json.JsonElement el)
                {
                    claims.Add(new Claim(kv.Key, el.ToString() ?? string.Empty));
                }
                else
                {
                    claims.Add(new Claim(kv.Key, kv.Value?.ToString() ?? string.Empty));
                }
            }
            return new ClaimsIdentity(claims, "jwt");
        }
        catch
        {
            return new ClaimsIdentity();
        }
    }

    private static string PadBase64(string base64)
    {
        return base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
    }
}
