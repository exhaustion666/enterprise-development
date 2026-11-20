namespace BeautySalon.Contracts.Interfaces;

/// <summary>
/// Класс публикации интеграционных событий
/// </summary>
public interface IEntityEventPublisher
{
    /// <summary>
    /// Публикует событие создания, изменения или удаления сущности
    /// </summary>
    public Task PublishAsync(
        string entityName,
        int entityId,
        string operation,
        CancellationToken cancellationToken = default);
}