using System.Diagnostics;
using BlazorTemplate.Client.DevTools;

namespace BlazorTemplate.Client.App;

// Semua request/response dicatat ke console browser (F12), dan di Debug juga ke
// panel DevInspector supaya body-nya bisa dibaca tanpa membuka DevTools browser.
class LoggingHandler(ILogger<LoggingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("--> {Method} {Url}", request.Method, request.RequestUri);

#if DEBUG
        var started = Stopwatch.GetTimestamp();
        // Body request dibaca dulu: hasilnya ter-buffer, jadi handler tetap bisa mengirimnya.
        var requestBody = request.Content is null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken);
#endif

        try
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation("<-- {Status} {Url}", (int)response.StatusCode, request.RequestUri);
            }
            else
            {
                // Body dibaca di sini supaya bisa dicatat; isinya ter-buffer,
                // jadi ApiClient masih bisa membacanya lagi.
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError("<-- {Status} {Url}\n{Body}", (int)response.StatusCode, request.RequestUri, body);
            }

#if DEBUG
            // Hanya di Debug: body sukses ikut dibaca untuk panel, dan buffering-nya
            // tidak membebani build Release.
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            ApiLog.Add(request.Method.Method, request.RequestUri?.ToString() ?? "",
                (int)response.StatusCode, Stopwatch.GetElapsedTime(started).TotalMilliseconds,
                requestBody, responseBody);
#endif

            return response;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            // Hanya mencatat. Penerjemahan ke ApiException ada di ApiClient (satu tempat).
            logger.LogError(ex, "Gagal menghubungi {Url}. Backend mati atau CORS menolak origin ini.", request.RequestUri);
#if DEBUG
            ApiLog.Add(request.Method.Method, request.RequestUri?.ToString() ?? "", null,
                Stopwatch.GetElapsedTime(started).TotalMilliseconds, requestBody, null, ex.Message);
#endif
            throw;
        }
    }
}
