using BeautySalon.Contracts.Dtos;

namespace BeautySalon.Application.Mappers;

/// <summary>
/// Преобразование доменной сущности в DTO и обратно
/// </summary>
public interface IEntityMapper<TEntity, TDto, in TEditDto>
    where TEntity : class
    where TDto : class, IEntityDto
{
    /// <summary>
    /// Создаёт DTO для чтения
    /// </summary>
    public TDto ToDto(TEntity entity);
    /// <summary>
    /// Создаёт сущность из DTO
    /// </summary>
    public TEntity ToEntity(TEditDto dto, int id);
}