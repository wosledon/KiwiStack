using KiwiStack.Front;
using KiwiStack.Front.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using KiwiStack.Front.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Load API base url from wwwroot/appsettings.json at runtime
var bootstrapClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var appOptions = await bootstrapClient.GetFromJsonAsync<ApiOptions>("appsettings.json") ?? new ApiOptions();

builder.Services.AddSingleton(appOptions);

builder.Services.AddScoped<ITokenStorage, TokenStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthStateProvider>();
// Ensure ITokenProvider resolves to the same ApiAuthStateProvider instance
builder.Services.AddScoped<ITokenProvider>(sp => (ApiAuthStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

builder.Services.AddAuthorizationCore();

// HttpClient configured for API with auth handler
builder.Services.AddTransient<AuthMessageHandler>();

// Mock store + handler
builder.Services.AddSingleton<MockApiStore>();
builder.Services.AddTransient<MockApiHandler>();

builder.Services.AddScoped(sp =>
{
    var options = sp.GetRequiredService<ApiOptions>();
    var authHandler = sp.GetRequiredService<AuthMessageHandler>();

    DelegatingHandler pipeline = authHandler;

    // Enable mock when ApiBaseUrl is empty or equals "mock"
    if (string.IsNullOrWhiteSpace(options.ApiBaseUrl) || string.Equals(options.ApiBaseUrl, "mock", StringComparison.OrdinalIgnoreCase))
    {
        var mock = sp.GetRequiredService<MockApiHandler>();
        mock.InnerHandler = new HttpClientHandler();
        pipeline = mock; // no auth in mock path
    }

    var client = new HttpClient(pipeline)
    {
        BaseAddress = string.IsNullOrWhiteSpace(options.ApiBaseUrl) || string.Equals(options.ApiBaseUrl, "mock", StringComparison.OrdinalIgnoreCase)
            ? new Uri("/")
            : new Uri(options.ApiBaseUrl.TrimEnd('/'))
    };
    return client;
});

builder.Services.AddScoped<ApiClient>(sp =>
{
    var client = sp.GetRequiredService<HttpClient>();
    var options = sp.GetRequiredService<ApiOptions>();
    var api = new ApiClient(client, Options.Create(options));
    api.ConfigureBaseAddress();
    return api;
});

// services
builder.Services.AddScoped<ProjectService>();

// mudblazor
builder.Services.AddMudServices();

await builder.Build().RunAsync();
