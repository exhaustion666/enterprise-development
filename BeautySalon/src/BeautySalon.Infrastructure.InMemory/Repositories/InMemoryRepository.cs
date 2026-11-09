using BeautySalon.Domain.Exceptions;
using BeautySalon.Domain.Interfaces;

namespace BeautySalon.Infrastructure.InMemory.Repositories;

/// <summary>
/// Репозиторий, хранящий сущности в словаре в памяти
/// </summary>
public class InMemoryRepository<TEntity> : IRepository<TEntity, int>
    where TEntity : class, IEntity
{
    private readonly Lock _sync = new();
    private readonly Dictionary<int, TEntity> _entities;
    private readonly Action<TEntity, TEntity> _updateEntity;
    private readonly Func<TEntity, bool>? _isReferenced;
    private int _lastId;

    /// <summary>
    /// Создаёт репозиторий и загружает начальные данные
    /// </summary>
    public InMemoryRepository(
        IEnumerable<TEntity> entities,
        Action<TEntity, TEntity> updateEntity,
        Func<TEntity, bool>? isReferenced = null)
    {
        ArgumentNullException.ThrowIfNull(entities);
        ArgumentNullException.ThrowIfNull(updateEntity);

        _entities = entities.ToDictionary(entity => entity.Id);
        _lastId = _entities.Count == 0 ? 0 : _entities.Keys.Max();
        _updateEntity = updateEntity;
        _isReferenced = isReferenced;
    }

    /// <summary>
    /// Возвращает все сущности в порядке идентификаторов
    /// </summary>
    public virtual Task<IReadOnlyList<TEntity>> GetAllAsync()
    {
        lock (_sync)
        {
            return Task.FromResult<IReadOnlyList<TEntity>>(
                [.. _entities.Values.OrderBy(entity => entity.Id)]);
        }
    }

    /// <summary>
    /// Возвращает сущность по идентификатору
    /// </summary>
    public virtual Task<TEntity?> GetByIdAsync(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(id);

        lock (_sync)
        {
            return Task.FromResult(_entities.GetValueOrDefault(id));
        }
    }

    /// <summary>
    /// Добавляет сущность и назначает следующий идентификатор
    /// </summary>
    public virtual Task<TEntity> AddAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        lock (_sync)
        {
            entity.Id = checked(++_lastId);
            _entities.Add(entity.Id, entity);

            return Task.FromResult(entity);
        }
    }

    /// <summary>
    /// Обновляет существующую сущность
    /// </summary>
    public virtual Task<TEntity?> UpdateAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentOutOfRangeException.ThrowIfNegative(entity.Id);

        lock (_sync)
        {
            if (!_entities.TryGetValue(entity.Id, out TEntity? existing) || existing is null)
            {
                return Task.FromResult<TEntity?>(null);
            }

            _updateEntity(existing, entity);

            return Task.FromResult<TEntity?>(existing);
        }
    }

    /// <summary>
    /// Удаляет сущность
    /// </summary>
    public virtual Task<bool> DeleteAsync(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(id);

        lock (_sync)
        {
            return !_entities.TryGetValue(id, out TEntity? entity)
                ? Task.FromResult(false)
                : _isReferenced?.Invoke(entity) == true
                ? throw new EntityConflictException(
                    "Нельзя удалить сущность, пока на неё ссылаются другие записи.")
                : Task.FromResult(_entities.Remove(id));
        }
    }

    /// <summary>
    /// Проверяет наличие сущности по условию
    /// </summary>
    protected bool Any(Func<TEntity, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        lock (_sync)
        {
            return _entities.Values.Any(predicate);
        }
    }
}