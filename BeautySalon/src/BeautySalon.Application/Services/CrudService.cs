using BeautySalon.Application.Mappers;
using BeautySalon.Contracts.Dtos;
using BeautySalon.Contracts.Interfaces;
using BeautySalon.Domain.Interfaces;

namespace BeautySalon.Application.Services;

/// <summary>
/// Общая реализация CRUD-сервиса
/// </summary>
public sealed class CrudService<TEntity, TDto, TEditDto>(
    IRepository<TEntity, int> repository,
    IEntityMapper<TEntity, TDto, TEditDto> mapper) : ICrudService<TDto, TEditDto>
    where TEntity : class
    where TDto : class, IEntityDto
{
    /// <summary>
    /// Возвращает DTO всех сущностей
    /// </summary>
    public async Task<IReadOnlyList<TDto>> GetAllAsync()
    {
        IReadOnlyList<TEntity> entities = await repository.GetAllAsync();
        return [.. entities.Select(mapper.ToDto)];
    }

    /// <summary>
    /// Возвращает DTO сущности
    /// </summary>
    public async Task<TDto?> GetByIdAsync(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(id);
        TEntity? entity = await repository.GetByIdAsync(id);
        return entity is null ? null : mapper.ToDto(entity);
    }

    /// <summary>
    /// Создаёт сущность через репозиторий
    /// </summary>
    public async Task<TDto> CreateAsync(TEditDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        TEntity entity = mapper.ToEntity(dto, 0);
        TEntity created = await repository.AddAsync(entity);
        return mapper.ToDto(created);
    }

    /// <summary>
    /// Обновляет сущность
    /// </summary>
    public async Task<TDto?> UpdateAsync(int id, TEditDto dto)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(id);
        ArgumentNullException.ThrowIfNull(dto);
        TEntity? existing = await repository.GetByIdAsync(id);
        if (existing is null)
        {
            return null;
        }

        TEntity entity = mapper.ToEntity(dto, id);
        TEntity? updated = await repository.UpdateAsync(entity);
        return updated is null ? null : mapper.ToDto(updated);
    }

    /// <summary>
    /// Удаляет сущность
    /// </summary>
    public Task<bool> DeleteAsync(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(id);
        return repository.DeleteAsync(id);
    }
}