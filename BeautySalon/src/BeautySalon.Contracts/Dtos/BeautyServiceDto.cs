namespace BeautySalon.Contracts.Dtos;

/// <summary>
/// Класс услуги
/// </summary>
public sealed class BeautyServiceDto : IEntityDto
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Наименование
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Категория
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// Цена
    /// </summary>
    public required decimal Price { get; init; }

    /// <summary>
    /// Длительность
    /// </summary>
    public required TimeSpan Duration { get; init; }
}