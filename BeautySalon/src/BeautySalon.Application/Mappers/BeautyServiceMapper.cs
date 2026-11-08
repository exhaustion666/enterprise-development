using BeautySalon.Contracts.Dtos;
using BeautySalon.Domain.Entities;

namespace BeautySalon.Application.Mappers;

/// <summary>
/// Маппер услуг
/// </summary>
public sealed class BeautyServiceMapper : IEntityMapper<BeautyService, BeautyServiceDto, BeautyServiceEditDto>
{
    /// <summary>
    /// Преобразует услугу в DTO
    /// </summary>
    public BeautyServiceDto ToDto(BeautyService entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Category = entity.Category,
        Price = entity.Price,
        Duration = entity.Duration
    };

    /// <summary>
    /// Создаёт доменную услугу из входного DTO
    /// </summary>
    public BeautyService ToEntity(BeautyServiceEditDto dto, int id) => new()
    {
        Id = id,
        Name = MappingValidation.Required(dto.Name, nameof(dto.Name)),
        Category = MappingValidation.Required(dto.Category, nameof(dto.Category)),
        Price = MappingValidation.Required(dto.Price, nameof(dto.Price)),
        Duration = TimeSpan.FromMinutes(MappingValidation.Required(dto.DurationMinutes, nameof(dto.DurationMinutes)))
    };
}