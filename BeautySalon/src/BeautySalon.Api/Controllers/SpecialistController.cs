using BeautySalon.Contracts.Dtos;
using BeautySalon.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Api.Controllers;

/// <summary>
/// Управление специалистами салона красоты
/// </summary>
[ApiController]
[Route("api/specialists")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public sealed class SpecialistController(ICrudService<SpecialistDto, SpecialistEditDto> service, ILogger<SpecialistController> logger) : ControllerBase
{
    /// <summary>
    /// Возвращает всех специалистов
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SpecialistDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SpecialistDto>>> GetAllAsync() =>
        Ok(await service.GetAllAsync());

    /// <summary>
    /// Возвращает специалиста по идентификатору
    /// </summary>
    [HttpGet("{id:int}", Name = "SpecialistById")]
    [ProducesResponseType(typeof(SpecialistDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SpecialistDto>> GetByIdAsync(
        [FromRoute, Range(0, int.MaxValue)] int id)
    {
        SpecialistDto? specialist = await service.GetByIdAsync(id);
        if (specialist is null)
        {
            return NotFound();
        }

        return Ok(specialist);
    }

    /// <summary>
    /// Создаёт специалиста
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SpecialistDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SpecialistDto>> CreateAsync(
        [FromBody] SpecialistEditDto request)
    {
        SpecialistDto created = await service.CreateAsync(request);
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "Создан клиент с ID {CustomerId}",
                created.Id);
        }
        return CreatedAtRoute("SpecialistById", new { id = created.Id }, created);
    }

    /// <summary>
    /// Обновляет данные специалиста
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateAsync(
        [FromRoute, Range(0, int.MaxValue)] int id,
        [FromBody] SpecialistEditDto request)
    {
        SpecialistDto? updated = await service.UpdateAsync(id, request);

        if (updated is null)
        {
            logger.LogWarning("Специалист с ID {SpecialistId} не найден для обновления", id);
            return NotFound();
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Обновлен специалист с ID {SpecialistId}", id);
        }

        return NoContent();
    }

    /// <summary>
    /// Удаляет специалиста
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute, Range(0, int.MaxValue)] int id)
    {
        var deleted = await service.DeleteAsync(id);

        if (!deleted)
        {
            logger.LogWarning("Специалист с ID {SpecialistId} не найден для удаления", id);
            return NotFound();
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Удален специалист с ID {SpecialistId}", id);
        }

        return NoContent();
    }
}