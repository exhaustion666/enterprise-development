using BeautySalon.Contracts.Dtos;
using BeautySalon.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Api.Controllers;

/// <summary>
/// Управление клиентами салона красоты
/// </summary>
[ApiController]
[Route("api/customers")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public sealed class CustomerController(ICrudService<CustomerDto, CustomerEditDto> service, ILogger<CustomerController> logger) : ControllerBase
{
    /// <summary>
    /// Возвращает всех клиентов
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CustomerDto>>> GetAllAsync() =>
        Ok(await service.GetAllAsync());

    /// <summary>
    /// Возвращает клиента по идентификатору
    /// </summary>
    [HttpGet("{id:int}", Name = "CustomerById")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetByIdAsync(
        [FromRoute, Range(0, int.MaxValue)] int id)
    {
        CustomerDto? customer = await service.GetByIdAsync(id);
        if (customer is null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    /// <summary>
    /// Создаёт клиента
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerDto>> CreateAsync(
        [FromBody] CustomerEditDto request)
    {
        CustomerDto created = await service.CreateAsync(request);
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "Создан клиент с ID {CustomerId}",
                created.Id);
        }
        return CreatedAtRoute("CustomerById", new { id = created.Id }, created);
    }

    /// <summary>
    /// Обновляет данные клиента
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateAsync(
        [FromRoute, Range(0, int.MaxValue)] int id,
        [FromBody] CustomerEditDto request)
    {
        CustomerDto? updated = await service.UpdateAsync(id, request);

        if (updated is null)
        {
            logger.LogWarning("Клиент с ID {CustomerId} не найден для обновления", id);
            return NotFound();
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Обновлен клиент с ID {CustomerId}", id);
        }

        return NoContent();
    }

    /// <summary>
    /// Удаляет клиента
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
            logger.LogWarning("Клиент с ID {CustomerId} не найден для удаления", id);
            return NotFound();
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Удален клиент с ID {CustomerId}", id);
        }

        return NoContent();
    }
}