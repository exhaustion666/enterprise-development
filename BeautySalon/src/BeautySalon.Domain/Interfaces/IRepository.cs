namespace BeautySalon.Domain.Interfaces;

/// <summary>
/// Асинхронный контракт CRUD-операций для сущности
/// </summary>
public interface IRepository<TEntity, TKey>
{
    /// <summary>
    /// Возвращает все сущности
    /// </summary>
    public Task<IReadOnlyList<TEntity>> GetAllAsync();

    /// <summary>
    /// Возвращает сущность по идентификатору
    /// </summary>
    public Task<TEntity?> GetByIdAsync(TKey id);

    /// <summary>
    /// Добавляет сущность
    /// </summary>
    public Task<TEntity> AddAsync(TEntity entity);

    /// <summary>
    /// Обновляет сущность
    /// </summary>
    public Task<TEntity?> UpdateAsync(TEntity entity);

    /// <summary>
    /// Удаляет сущность
    /// </summary>
    public Task<bool> DeleteAsync(TKey id);
}