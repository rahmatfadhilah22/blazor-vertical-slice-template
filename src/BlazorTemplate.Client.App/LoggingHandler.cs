namespace BlazorTemplate.Client.App;

// Semua request/response dicatat ke console browser (F12), termasuk body error.
class LoggingHandler(ILogger<LoggingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("--> {Method} {Url}", request.Method, request.RequestUri);

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

            return response;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            // Hanya mencatat. Penerjemahan ke ApiException ada di ApiClient (satu tempat).
            logger.LogError(ex, "Gagal menghubungi {Url}. Backend mati atau CORS menolak origin ini.", request.RequestUri);
            throw;
        }
    }
}
