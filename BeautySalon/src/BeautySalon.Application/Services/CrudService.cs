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
    IEntityMapper<TEntity, TDto, TEditDto> mapper,
    IEntityEventPublisher eventPublisher) : ICrudService<TDto, TEditDto>
    where TEntity : class, IEntity
    where TDto : class, IEntityDto
{
    /// <summary>
    /// Возвращает DTO всех сущностей
    /// </summary>
    public async Task<IReadOnlyList<TDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await repository.GetAllAsync(cancellationToken);
        return [.. entities.Select(mapper.ToDto)];
    }

    /// <summary>
    /// Возвращает DTO сущности
    /// </summary>
    public async Task<TDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : mapper.ToDto(entity);
    }

    /// <summary>
    /// Создаёт сущность через репозиторий
    /// </summary>
    public async Task<TDto> CreateAsync(TEditDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var entity = mapper.ToEntity(dto, 0);
        var created = await repository.AddAsync(entity, cancellationToken);
        await eventPublisher.PublishAsync(typeof(TEntity).Name, created.Id, "created", cancellationToken);
        return mapper.ToDto(created);
    }

    /// <summary>
    /// Обновляет сущность
    /// </summary>
    public async Task<TDto?> UpdateAsync(
        int id, TEditDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(dto);
        var entity = mapper.ToEntity(dto, id);
        var updated = await repository.UpdateAsync(entity, cancellationToken);

        if (updated is null)
        {
            return null;
        }

        await eventPublisher.PublishAsync(typeof(TEntity).Name, updated.Id, "updated", cancellationToken);
        return mapper.ToDto(updated);
    }

    /// <summary>
    /// Удаляет сущность
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        var deleted = await repository.DeleteAsync(id, cancellationToken);

        if (deleted)
        {
            await eventPublisher.PublishAsync(typeof(TEntity).Name, id, "deleted", cancellationToken);
        }

        return deleted;
    }
}