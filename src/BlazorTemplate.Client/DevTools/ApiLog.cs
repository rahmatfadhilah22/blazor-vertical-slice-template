using System.Text.Json;

namespace BlazorTemplate.Client.DevTools;

/// <summary>
/// Riwayat request/response API untuk panel DevInspector (DEBUG saja).
/// Disimpan di memori tab, maksimal <see cref="Max"/> entri terbaru — tidak pernah
/// dikirim ke mana pun dan hilang saat refresh.
/// </summary>
public static class ApiLog
{
    private const int Max = 30;
    private const int MaxBodyChars = 8000;

    private static int _seq;

    /// <summary>Terbaru di urutan pertama, siap ditampilkan tanpa Reverse.</summary>
    public static List<Entry> Entries { get; } = [];

    /// <summary>Dipicu setelah entri ditambah/dibersihkan; panel mendengarkan untuk render ulang.</summary>
    public static event Action? Changed;

    public static void Add(string method, string url, int? status, double elapsedMs,
        string? request, string? response, string? error = null)
    {
        Entries.Insert(0, new Entry(++_seq, DateTime.Now, method, url, status,
            (long)elapsedMs, Trim(request), Trim(response), error));

        if (Entries.Count > Max)
            Entries.RemoveAt(Entries.Count - 1);

        Changed?.Invoke();
    }

    public static void Clear()
    {
        Entries.Clear();
        Changed?.Invoke();
    }

    /// <summary>JSON diindentasi kalau bisa diparse; selain itu dikembalikan apa adanya.</summary>
    public static string Pretty(string body)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (JsonException)
        {
            return body;
        }
    }

    private static string? Trim(string? body)
        => body is null || body.Length <= MaxBodyChars ? body : $"{body[..MaxBodyChars]}\n… (dipotong)";

    public sealed record Entry(int Seq, DateTime At, string Method, string Url,
        int? Status, long ElapsedMs, string? Request, string? Response, string? Error);
}
