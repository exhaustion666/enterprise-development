using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Api.Middleware;

/// <summary>
/// Преобразует ошибки приложения и базы данных в HTTP-ответы
/// </summary>
public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    /// <summary>
    /// Формирует HTTP-ответ
    /// </summary>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var postgresException = FindPostgresException(exception);
        var (status, title, detail) = postgresException?.SqlState switch
        {
            PostgresErrorCodes.ForeignKeyViolation or
            PostgresErrorCodes.RestrictViolation => (
                StatusCodes.Status409Conflict,
                "Конфликт связанных данных",
                "Нельзя удалить или изменить запись, на которую ссылаются другие записи."),
            PostgresErrorCodes.UniqueViolation => (
                StatusCodes.Status409Conflict,
                "Конфликт уникальности",
                "Запись с таким уникальным значением уже существует."),
            PostgresErrorCodes.CheckViolation or
            PostgresErrorCodes.NotNullViolation or
            PostgresErrorCodes.StringDataRightTruncation or
            PostgresErrorCodes.NumericValueOutOfRange => (
                StatusCodes.Status400BadRequest,
                "Некорректные данные",
                "Одно или несколько значений не соответствуют ограничениям базы данных."),
            _ => MapGeneralException(exception)
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                exception,
                "Ошибка при обработке {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);

            detail = "Не удалось обработать запрос. Повторите попытку позже.";
        }
        else
        {
            logger.LogWarning(
                exception,
                "Запрос {Method} {Path} завершён с кодом {Status}: {Title}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                status,
                title);
        }

        ProblemDetails problem = status == StatusCodes.Status400BadRequest
            ? new ValidationProblemDetails { Title = title }
            : new ProblemDetails { Title = title, Detail = detail };

        problem.Status = status;
        problem.Instance = httpContext.Request.Path;
        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        if (problem is ValidationProblemDetails validationProblem)
        {
            validationProblem.Errors.Add("request", [detail]);
        }

        httpContext.Response.StatusCode = status;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(
            problem,
            options: null,
            contentType: "application/problem+json",
            cancellationToken: cancellationToken);

        return true;
    }

    /// <summary>
    /// Ищет исключение базы данных внутри цепочки исключений
    /// </summary>
    private static PostgresException? FindPostgresException(Exception exception)
    {
        for (Exception? current = exception;
             current is not null;
             current = current.InnerException)
        {
            if (current is PostgresException postgresException)
            {
                return postgresException;
            }
        }

        return null;
    }

    /// <summary>
    /// Преобразует общие исключения в подходящие статусы
    /// </summary>
    private static (int Status, string Title, string Detail) MapGeneralException(
        Exception exception)
    {
        return exception switch
        {
            DbUpdateConcurrencyException => (
                StatusCodes.Status409Conflict,
                "Конфликт изменения данных",
                "Запись была изменена или удалена другим запросом. Обновите данные и повторите попытку."),
            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Ресурс не найден",
                exception.Message),
            ArgumentException or ValidationException => (
                StatusCodes.Status400BadRequest,
                "Некорректные данные",
                exception.Message),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Внутренняя ошибка сервера",
                "Не удалось обработать запрос. Повторите попытку позже.")
        };
    }
}