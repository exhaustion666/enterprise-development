namespace BeautySalon.Domain.Entities;

/// <summary>
/// Класс услуги
/// </summary>
public class BeautyService
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Наименование
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Категория
    /// </summary>
    public required string Category { get; set; }

    /// <summary>
    /// Цена
    /// </summary>
    public required decimal Price { get; set; }

    /// <summary>
    /// Длительность 
    /// </summary>
    public required TimeSpan Duration { get; set; }
}
