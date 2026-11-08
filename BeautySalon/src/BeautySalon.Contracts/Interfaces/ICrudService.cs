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
    public Task<IReadOnlyList<TDto>> GetAllAsync();

    /// <summary>
    /// Возвращает запись по идентификатору
    /// </summary>
    public Task<TDto?> GetByIdAsync(int id);

    /// <summary>
    /// Создаёт запись
    /// </summary>
    public Task<TDto> CreateAsync(TEditDto dto);

    /// <summary>
    /// Изменяет запись
    /// </summary>
    public Task<TDto?> UpdateAsync(int id, TEditDto dto);

    /// <summary>
    /// Удаляет запись
    /// </summary>
    public Task<bool> DeleteAsync(int id);
}