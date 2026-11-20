using BeautySalon.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BeautySalon.Infrastructure.PostgreSql.Repositories;

/// <summary>
/// Реализация CRUD-репозитория
/// </summary>
public class PostgreSqlRepository<TEntity>(BeautySalonDbContext context)
    : IRepository<TEntity, int>
    where TEntity : class, IEntity
{
    /// <summary>
    /// Запрос для чтения без отслеживания сущностей
    /// </summary>
    protected virtual IQueryable<TEntity> Query => context.Set<TEntity>().AsNoTracking();

    /// <summary>
    /// Возвращает все сущности
    /// </summary>
    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await Query
            .OrderBy(entity => entity.Id)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Возвращает сущность по идентификатору
    /// </summary>
    public virtual Task<TEntity?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return Query.FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    /// <summary>
    /// Добавляет сущность и возвращает сохранённый объект
    /// </summary>
    public virtual async Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await context.Set<TEntity>().AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <summary>
    /// Изменяет только поля и связи уже существующей записи
    /// </summary>
    public virtual async Task<TEntity?> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var existing = await context.Set<TEntity>().FindAsync(
            [entity.Id], cancellationToken);

        if (existing is null)
        {
            return null;
        }

        context.Entry(existing).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(entity.Id, cancellationToken);
    }

    /// <summary>
    /// Удаляет сущность
    /// </summary>
    public virtual async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var existing = await context.Set<TEntity>().FindAsync(
            [id], cancellationToken);

        if (existing is null)
        {
            return false;
        }

        context.Set<TEntity>().Remove(existing);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}