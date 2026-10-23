using BlazorTemplate.Client;
using BlazorTemplate.Client.App;
using BlazorTemplate.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddRadzenComponents();

// Pastikan semua log level Information ke atas tampil di console browser.
builder.Logging.SetMinimumLevel(LogLevel.Information);

var apiBaseUrl = builder.Configuration["Api:BaseUrl"]
    ?? throw new InvalidOperationException("Api:BaseUrl belum diisi di wwwroot/appsettings.json");

// HttpClient hanya menggabungkan path relatif dengan benar kalau base berakhiran '/'.
if (!apiBaseUrl.EndsWith('/'))
    apiBaseUrl += "/";

builder.Services.AddScoped(sp => new HttpClient(
    new LoggingHandler(sp.GetRequiredService<ILogger<LoggingHandler>>())
    {
        InnerHandler = new HttpClientHandler(),
    })
    { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddScoped<ApiClient>();

await builder.Build().RunAsync();
