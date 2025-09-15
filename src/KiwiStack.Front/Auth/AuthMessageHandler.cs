using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;

namespace KiwiStack.Front.Auth;

public class AuthMessageHandler(ITokenProvider tokenProvider, NavigationManager nav) : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly NavigationManager _nav = nav;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenProvider.GetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // redirect to login
            _nav.NavigateTo("/login", forceLoad: true);
        }
        return response;
    }
}

public interface ITokenProvider
{
    Task<string?> GetTokenAsync();
}
