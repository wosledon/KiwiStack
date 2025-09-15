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
        if (!string.IsNullOrWhiteSpace(options.Value.ApiBaseUrl))
        {
            Http.BaseAddress = new Uri(options.Value.ApiBaseUrl.TrimEnd('/'));
        }
    }

    public async Task<T?> GetAsync<T>(string url, CancellationToken ct = default)
        => await Http.GetFromJsonAsync<T>(url, ct);

    public async Task<HttpResponseMessage> PostAsync<T>(string url, T body, CancellationToken ct = default)
        => await Http.PostAsJsonAsync(url, body, ct);
}
