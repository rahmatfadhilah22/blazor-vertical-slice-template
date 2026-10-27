using BlazorTemplate.Client;
using BlazorTemplate.Client.App;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Localization;
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

// Satu instance untuk seluruh aplikasi: bahasa adalah state global, dan komponen
// mendengarkan Localizer.Changed untuk render ulang.
builder.Services.AddSingleton<Localizer>();

#if DEBUG
// Perubahan pertama setelah tambah bahasa biasanya lupa satu kunci di salah satu dictionary.
// Gagal keras saat start jauh lebih cepat ketahuan daripada teks mentah di layar.
if (Localizer.FindMissingKeys() is { Count: > 0 } missing)
    throw new InvalidOperationException($"Kunci terjemahan tidak lengkap: {string.Join(", ", missing)}");
#endif

await builder.Build().RunAsync();
