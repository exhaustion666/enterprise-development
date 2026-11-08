using BeautySalon.Contracts.Dtos;
using BeautySalon.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Api.Controllers;

/// <summary>
/// Класс аналитики по записям салона
/// </summary>
[ApiController]
[Route("api/analytics")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public sealed class AnalyticsController(
    ISalonAnalyticsService analyticsService,
    ILogger<AnalyticsController> logger) : ControllerBase
{
    /// <summary>
    /// Мастера с заданным минимальным стажем
    /// </summary>
    [HttpGet("experienced-specialists")]
    [ProducesResponseType(typeof(IReadOnlyList<SpecialistDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<SpecialistDto>>> GetExperiencedSpecialistsAsync(
        [FromQuery, Range(0, 80)] int minimumExperienceYears = 5)
    {
        var specialists = await analyticsService.GetExperiencedSpecialistsAsync(minimumExperienceYears);

        return Ok(specialists);
    }

    /// <summary>
    /// Свободные "окошки" у мастера
    /// </summary>
    [HttpGet("specialist-windows/{specialistId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<TimeSlotDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<TimeSlotDto>>> GetSpecialistWindowsAsync(
        [FromRoute, Range(0, int.MaxValue)] int specialistId)
    {
        var windows = await analyticsService.GetSpecialistWindowsAsync(specialistId);

        if (windows is null)
        {
            logger.LogWarning(
                "Специалист с ID {SpecialistId} не найден при запросе свободных окон.",
                specialistId);

            return NotFound();
        }

        return Ok(windows);
    }

    /// <summary>
    /// Самые популярные услуги
    /// </summary>
    [HttpGet("top-services")]
    [ProducesResponseType(typeof(IReadOnlyList<PopularServiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<PopularServiceDto>>> GetTopServicesAsync(
        [FromQuery, Range(1, 100)] int count = 5)
    {
        var services = await analyticsService.GetTopServicesAsync(count);

        return Ok(services);
    }

    /// <summary>
    /// Количество повторных записей клиентов за указанный период
    /// </summary>
    [HttpGet("repeat-bookings")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> GetRepeatBookingCountAsync(
        [FromQuery, Required] DateTimeOffset? from,
        [FromQuery, Required] DateTimeOffset? to)
    {
        if (from is null || to is null || from.Value >= to.Value)
        {
            ValidationProblemDetails problem = new()
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Некорректный период"
            };

            problem.Errors.Add(
                "period",
                ["Параметры from и to обязательны; from должен быть раньше to."]);

            return ValidationProblem(problem);
        }

        var result = await analyticsService.GetRepeatBookingCountAsync(
            from.Value,
            to.Value);

        return Ok(result);
    }

    /// <summary>
    /// Клиенты записанные к нескольким мастерам упорядоченные по дате рождения
    /// </summary>
    [HttpGet("customers-with-multiple-specialists")]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CustomerDto>>> GetCustomersWithMultipleSpecialistsAsync()
    {
        var customers = await analyticsService.GetCustomersWithMultipleSpecialistsAsync();

        return Ok(customers);
    }
}