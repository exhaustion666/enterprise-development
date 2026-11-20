namespace BeautySalon.Domain.Interfaces;

/// <summary>
/// Асинхронный контракт CRUD-операций для сущности
/// </summary>
public interface IRepository<TEntity, TKey>
{
    /// <summary>
    /// Возвращает все сущности
    /// </summary>
    public Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает сущность по идентификатору
    /// </summary>
    public Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет сущность
    /// </summary>
    public Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет сущность
    /// </summary>
    public Task<TEntity?> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет сущность
    /// </summary>
    public Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
}