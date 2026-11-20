using BeautySalon.Contracts.Dtos;

namespace BeautySalon.Contracts.Interfaces;

/// <summary>
/// Контракт асинхронного CRUD-сервиса
/// </summary>
public interface ICrudService<TDto, in TEditDto> where TDto : class, IEntityDto
{
    /// <summary>
    /// Возвращает все записи
    /// </summary>
    public Task<IReadOnlyList<TDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает запись по идентификатору
    /// </summary>
    public Task<TDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт запись
    /// </summary>
    public Task<TDto> CreateAsync(TEditDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Изменяет запись
    /// </summary>
    public Task<TDto?> UpdateAsync(int id, TEditDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет запись
    /// </summary>
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}