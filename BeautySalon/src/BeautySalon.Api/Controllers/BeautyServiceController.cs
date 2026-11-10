using BeautySalon.Contracts.Dtos;
using BeautySalon.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Api.Controllers;

/// <summary>
/// Управление услугами салона красоты
/// </summary>
[ApiController]
[Route("api/beauty-services")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public sealed class BeautyServiceController(ICrudService<BeautyServiceDto, BeautyServiceEditDto> service, ILogger<BeautyServiceController> logger) : ControllerBase
{
    /// <summary>
    /// Возвращает все услуги
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BeautyServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BeautyServiceDto>>> GetAllAsync()
    {
        return Ok(await service.GetAllAsync());
    }

    /// <summary>
    /// Возвращает услугу по идентификатору
    /// </summary>
    [HttpGet("{id:int}", Name = "BeautyServiceById")]
    [ProducesResponseType(typeof(BeautyServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BeautyServiceDto>> GetByIdAsync(
        [FromRoute, Range(1, int.MaxValue)] int id)
    {
        var beautyService = await service.GetByIdAsync(id);
        return beautyService is null ? (ActionResult<BeautyServiceDto>)NotFound() : (ActionResult<BeautyServiceDto>)Ok(beautyService);
    }

    /// <summary>
    /// Создаёт услугу
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(BeautyServiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BeautyServiceDto>> CreateAsync(
        [FromBody] BeautyServiceEditDto request)
    {
        var created = await service.CreateAsync(request);
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "Создана услуга с ID {BeautyServiceId}",
                created.Id);
        }
        return CreatedAtRoute("BeautyServiceById", new { id = created.Id }, created);
    }

    /// <summary>
    /// Обновляет данные услуги
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateAsync(
        [FromRoute, Range(1, int.MaxValue)] int id,
        [FromBody] BeautyServiceEditDto request)
    {
        var updated = await service.UpdateAsync(id, request);

        if (updated is null)
        {
            logger.LogWarning("Услуга с ID {BeautyServiceId} не найдена для обновления", id);
            return NotFound();
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Обновлена услуга с ID {BeautyServiceId}", id);
        }

        return NoContent();
    }

    /// <summary>
    /// Удаляет услугу
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute, Range(1, int.MaxValue)] int id)
    {
        var deleted = await service.DeleteAsync(id);

        if (!deleted)
        {
            logger.LogWarning("Услуга с ID {BeautyServiceId} не найдена для удаления", id);
            return NotFound();
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Удалена услуга с ID {BeautyServiceId}", id);
        }

        return NoContent();
    }
}