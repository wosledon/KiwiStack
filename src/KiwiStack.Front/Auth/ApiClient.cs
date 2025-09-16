using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace KiwiStack.Front.Auth;

public class ApiOptions
{
    public string ApiBaseUrl { get; set; } = string.Empty;
}

public class ApiClient(HttpClient http, IOptions<ApiOptions> options)
{
    public HttpClient Http { get; } = http;

    public void ConfigureBaseAddress()
    {
        var raw = options.Value.ApiBaseUrl?.Trim() ?? string.Empty;
        // Skip when mock or invalid
        if (string.Equals(raw, "mock", StringComparison.OrdinalIgnoreCase)) return;
        if (Uri.TryCreate(raw.TrimEnd('/'), UriKind.Absolute, out var uri))
        {
            Http.BaseAddress = uri;
        }
    }

    public async Task<T?> GetAsync<T>(string url, CancellationToken ct = default)
        => await Http.GetFromJsonAsync<T>(url, ct);

    public async Task<HttpResponseMessage> PostAsync<T>(string url, T body, CancellationToken ct = default)
        => await Http.PostAsJsonAsync(url, body, ct);
}
