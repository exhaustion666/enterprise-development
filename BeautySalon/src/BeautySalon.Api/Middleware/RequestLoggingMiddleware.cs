using System.Diagnostics;

namespace BeautySalon.Api.Middleware;

/// <summary>
/// Записывает HTTP-запросы и время их обработки
/// </summary>
public sealed class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger)
{
    /// <summary>
    /// Передаёт запрос следующему обработчику и записывает результат
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var start = Stopwatch.GetTimestamp();

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "Запрос {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
        }

        try
        {
            await next(context);
        }

        finally
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Ответ {Method} {Path}: {StatusCode}, {ElapsedMilliseconds:F1} мс",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    Stopwatch.GetElapsedTime(start).TotalMilliseconds);
            }
        }
    }
}