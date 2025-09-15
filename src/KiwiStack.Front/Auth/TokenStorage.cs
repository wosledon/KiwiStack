using Microsoft.JSInterop;

namespace KiwiStack.Front.Auth;

public interface ITokenStorage
{
    Task SetTokenAsync(string? token);
    Task<string?> GetTokenAsync();
    Task RemoveTokenAsync();
}

public class TokenStorage(IJSRuntime js) : ITokenStorage
{
    private const string Key = "auth_token";
    private readonly IJSRuntime _js = js;

    public async Task SetTokenAsync(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            await RemoveTokenAsync();
            return;
        }
        await _js.InvokeVoidAsync("localStorage.setItem", Key, token);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _js.InvokeAsync<string?>("localStorage.getItem", Key);
    }

    public async Task RemoveTokenAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", Key);
    }
}
