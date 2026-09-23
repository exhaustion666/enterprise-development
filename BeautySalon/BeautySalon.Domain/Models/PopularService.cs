namespace BeautySalon.Domain.Models;

/// <summary>
/// Класс самой популярной услуги
/// </summary>
public class PopularService
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public required int BeautyServiceId { get; set; }

    /// <summary>
    /// Количество записей на услугу
    /// </summary>
    public required int BookingCount { get; set; }
}
