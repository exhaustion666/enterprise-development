using BeautySalon.Contracts.Dtos;
using BeautySalon.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Api.Controllers;

/// <summary>
/// Управление записями клиентов на услуги салона красоты
/// </summary>
[ApiController]
[Route("api/bookings")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public sealed class BookingController(ICrudService<BookingDto, BookingEditDto> service, ILogger<BookingController> logger) : ControllerBase
{
    /// <summary>
    /// Возвращает все записи
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BookingDto>>> GetAllAsync()
    {
        return Ok(await service.GetAllAsync());
    }

    /// <summary>
    /// Возвращает запись по идентификатору
    /// </summary>
    [HttpGet("{id:int}", Name = "BookingById")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> GetByIdAsync(
        [FromRoute, Range(1, int.MaxValue)] int id)
    {
        var booking = await service.GetByIdAsync(id);
        return booking is null ? (ActionResult<BookingDto>)NotFound() : (ActionResult<BookingDto>)Ok(booking);
    }

    /// <summary>
    /// Создаёт запись клиента
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookingDto>> CreateAsync(
        [FromBody] BookingEditDto request)
    {
        var created = await service.CreateAsync(request);
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "Создана запись с ID {BookingId}",
                created.Id);
        }
        return CreatedAtRoute("BookingById", new { id = created.Id }, created);
    }

    /// <summary>
    /// Обновляет существующую запись
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateAsync(
        [FromRoute, Range(1, int.MaxValue)] int id,
        [FromBody] BookingEditDto request)
    {
        var updated = await service.UpdateAsync(id, request);

        if (updated is null)
        {
            logger.LogWarning("Запись с ID {BookingId} не найдена для обновления", id);
            return NotFound();
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Обновлена запись с ID {BookingId}", id);
        }

        return NoContent();
    }

    /// <summary>
    /// Удаляет запись
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute, Range(1, int.MaxValue)] int id)
    {
        var deleted = await service.DeleteAsync(id);

        if (!deleted)
        {
            logger.LogWarning("Запись с ID {BookingId} не найдена для удаления", id);
            return NotFound();
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Удалена запись с ID {BookingId}", id);
        }

        return NoContent();
    }
}