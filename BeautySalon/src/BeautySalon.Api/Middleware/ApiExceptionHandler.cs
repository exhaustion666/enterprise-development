using BeautySalon.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Api.Middleware;

/// <summary>
/// Преобразует ожидаемые исключения в единый формат
/// </summary>
public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    /// <summary>
    /// Формирует HTTP-ответ на основе типа исключения
    /// </summary>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Ресурс не найден"),
            EntityConflictException => (StatusCodes.Status409Conflict, "Конфликт данных"),
            ValidationException => (StatusCodes.Status400BadRequest, "Некорректные данные"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Некорректные данные"),
            _ => (StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера")
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Ошибка при обработке {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            logger.LogWarning("Запрос {Method} {Path} завершён с кодом {Status}: {Message}",
                httpContext.Request.Method, httpContext.Request.Path, status, exception.Message);
        }

        ProblemDetails problem;

        if (status == StatusCodes.Status400BadRequest)
        {
            ValidationProblemDetails validationProblem = new();
            validationProblem.Errors.Add("request", [exception.Message]);
            problem = validationProblem;
        }
        else
        {
            problem = new ProblemDetails
            {
                Detail = status == StatusCodes.Status500InternalServerError
                    ? "Не удалось обработать запрос. Повторите попытку позже."
                    : exception.Message
            };
        }

        problem.Status = status;
        problem.Title = title;
        problem.Instance = httpContext.Request.Path;
        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = status;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json", cancellationToken: cancellationToken);
        return true;
    }
}