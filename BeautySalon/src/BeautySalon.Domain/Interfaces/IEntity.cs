namespace BeautySalon.Domain.Interfaces;

/// <summary>
/// Общий контракт сущности с идентификатором
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Идентификатор сущности
    /// </summary>
    public int Id { get; set; }
}