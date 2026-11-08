namespace BeautySalon.Contracts.Dtos;

/// <summary>
/// Класс самой популярной услуги
/// </summary>
public sealed class PopularServiceDto
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public required int BeautyServiceId { get; init; }

    /// <summary>
    /// Название услуги
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Количество записей на услугу
    /// </summary>
    public required int BookingCount { get; init; }
}