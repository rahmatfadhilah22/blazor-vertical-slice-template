using System.Net.Http.Json;
using System.Text.Json;

namespace BlazorTemplate.Client.Services;

/// <summary>
/// Satu-satunya jalur HTTP fitur ke backend. Semua method:
/// - mengembalikan body bertipe pada sukses,
/// - melempar <see cref="ApiException"/> pada gagal (HTTP error, body kosong, atau backend tak terjangkau).
/// Jadi pemanggil hanya perlu menangani satu jenis exception.
/// </summary>
public class ApiClient(HttpClient http)
{
    public async Task<T> GetAsync<T>(string url)
    {
        using var response = await SendAsync(() => http.GetAsync(url));
        return await ReadJsonAsync<T>(response);
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

    public async Task DeleteAsync(string url, object body)
    {
        // Backend Categories/Delete membaca ids dari body JSON, bukan query string
        // (diukur langsung: tanpa Content-Type json responsnya 415).
        using var request = new HttpRequestMessage(HttpMethod.Delete, url) { Content = JsonContent.Create(body) };
        using var response = await SendAsync(() => http.SendAsync(request));
        await EnsureSuccessAsync(response);
    }

    private static async Task<HttpResponseMessage> SendAsync(Func<Task<HttpResponseMessage>> send)
    {
        try
        {
            return await send();
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException("Tidak bisa menghubungi server. Backend mati atau CORS menolak origin ini.", inner: ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new ApiException("Permintaan ke server timeout.", inner: ex);
        }
    }

    private static async Task<T> ReadJsonAsync<T>(HttpResponseMessage response)
    {
        await EnsureSuccessAsync(response);

        T? value;
        try
        {
            value = await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            throw new ApiException("Respons server tidak bisa dibaca.", inner: ex);
        }

        return value ?? throw new ApiException("Respons kosong dari server.");
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        ProblemDetails? problem = null;
        try { problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(); }
        catch (Exception ex) when (ex is JsonException or NotSupportedException) { /* bukan ProblemDetails */ }

        throw new ApiException(
            problem?.Title ?? $"Request gagal ({(int)response.StatusCode} {response.StatusCode}).",
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
