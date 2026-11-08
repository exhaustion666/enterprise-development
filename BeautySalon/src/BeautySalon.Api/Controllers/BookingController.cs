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
    public async Task<ActionResult<IReadOnlyList<BookingDto>>> GetAllAsync() =>
        Ok(await service.GetAllAsync());

    /// <summary>
    /// Возвращает запись по идентификатору
    /// </summary>
    [HttpGet("{id:int}", Name = "BookingById")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> GetByIdAsync(
        [FromRoute, Range(0, int.MaxValue)] int id)
    {
        BookingDto? booking = await service.GetByIdAsync(id);
        if (booking is null)
        {
            return NotFound();
        }

        return Ok(booking);
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
        BookingDto created = await service.CreateAsync(request);
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "Создан клиент с ID {CustomerId}",
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
        [FromRoute, Range(0, int.MaxValue)] int id,
        [FromBody] BookingEditDto request)
    {
        BookingDto? updated = await service.UpdateAsync(id, request);

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
        [FromRoute, Range(0, int.MaxValue)] int id)
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