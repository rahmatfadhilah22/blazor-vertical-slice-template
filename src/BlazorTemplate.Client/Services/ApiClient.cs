using System.Net.Http.Json;
using System.Text.Json;
using BlazorTemplate.Client.Models;
using BlazorTemplate.Shared.Localization;

namespace BlazorTemplate.Client.Services;

/// <summary>
/// Satu-satunya jalur HTTP fitur ke backend. Semua method:
/// - mengembalikan body bertipe pada sukses,
/// - melempar <see cref="ApiException"/> pada gagal (HTTP error, body kosong, atau backend tak terjangkau).
/// Jadi pemanggil hanya perlu menangani satu jenis exception.
///
/// Pesan bawaan (backend mati, timeout, respons kosong) diambil dari <see cref="Localizer"/>,
/// jadi ikut berganti bahasa seperti teks UI lain.
/// </summary>
public class ApiClient(HttpClient http, Localizer L)
{
    public async Task<T> GetAsync<T>(string url)
    {
        return await GetAsync<T>(url, query: null);
    }

    /// <summary>
    /// GET dengan query string dari object (properti PascalCase → camelCase, null dilewati).
    /// Nilai di-escape otomatis, jadi pemanggil tidak menulis <see cref="Uri.EscapeDataString"/>.
    /// </summary>
    public async Task<T> GetAsync<T>(string url, object? query)
    {
        using var response = await SendAsync(() => http.GetAsync(WithQuery(url, query)));
        return await ReadJsonAsync<T>(response);
    }

    /// <summary>
    /// GET endpoint berpaging: <paramref name="path"/> adalah path lengkap
    /// (mis. "Categories/GetRows"), karena nama action tiap backend bisa berbeda.
    /// <paramref name="search"/> kosong/spasi tidak dikirim, jadi pemanggil tidak perlu menormalkan.
    /// </summary>
    public Task<PagedResult<T>> GetPagedAsync<T>(string path, int page, int pageSize, string? search = null)
        => GetAsync<PagedResult<T>>(path, new
        {
            page,
            pageSize,
            search = string.IsNullOrWhiteSpace(search) ? null : search,
        });

    private static string WithQuery(string url, object? query)
    {
        if (query is null)
            return url;

        var pairs = query.GetType().GetProperties()
            .Select(p => (p.Name, Value: p.GetValue(query)))
            .Where(p => p.Value is not null)
            .Select(p => $"{char.ToLowerInvariant(p.Name[0])}{p.Name[1..]}={Uri.EscapeDataString(p.Value!.ToString()!)}");

        return $"{url}?{string.Join("&", pairs)}";
    }

    public async Task<T> PostAsync<T>(string url, object body)
    {
        using var response = await SendAsync(() => http.PostAsJsonAsync(url, body));
        return await ReadJsonAsync<T>(response);
    }

    public async Task PutAsync(string url, object body)
    {
        using var response = await SendAsync(() => http.PutAsJsonAsync(url, body));
        await EnsureSuccessAsync(response);
    }

    /// <summary>
    /// DELETE massal. Backend membaca <c>?ids=1&amp;ids=2</c> dari query
    /// (<c>[FromQuery] int[] ids</c>), bukan body JSON.
    /// </summary>
    public async Task DeleteAsync<T>(string url, IEnumerable<T> ids)
    {
        var query = string.Join("&", ids.Select(id => $"ids={Uri.EscapeDataString(id?.ToString() ?? "")}"));
        using var response = await SendAsync(() => http.DeleteAsync($"{url}?{query}"));
        await EnsureSuccessAsync(response);
    }

    private async Task<HttpResponseMessage> SendAsync(Func<Task<HttpResponseMessage>> send)
    {
        try
        {
            return await send();
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException(L["api.unreachable"], inner: ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new ApiException(L["api.timeout"], inner: ex);
        }
    }

    private async Task<T> ReadJsonAsync<T>(HttpResponseMessage response)
    {
        await EnsureSuccessAsync(response);

        T? value;
        try
        {
            value = await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            throw new ApiException(L["api.badResponse"], inner: ex);
        }

        return value ?? throw new ApiException(L["api.emptyResponse"]);
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        ProblemDetails? problem = null;
        try { problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(); }
        catch (Exception ex) when (ex is JsonException or NotSupportedException) { /* bukan ProblemDetails */ }

        throw new ApiException(
            problem?.Title ?? L.Format("api.requestFailed", (int)response.StatusCode),
            problem?.Errors,
            (int)response.StatusCode);
    }
}

public class ApiException(string message, Dictionary<string, string[]>? errors = null,
    int? statusCode = null, Exception? inner = null) : Exception(message, inner)
{
    public Dictionary<string, string[]> Errors { get; } = errors ?? [];
    public int? StatusCode { get; } = statusCode;
}

// Bentuk RFC 9457 yang dipakai backend; errors hanya ada pada 400 validasi.
public class ProblemDetails
{
    public int? Status { get; set; }
    public string? Title { get; set; }
    public string? Instance { get; set; }
    public string? TraceId { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
}
