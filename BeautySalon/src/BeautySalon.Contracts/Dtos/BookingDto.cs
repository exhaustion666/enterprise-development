namespace BeautySalon.Contracts.Dtos;

/// <summary>
/// Класс записи на услугу
/// </summary>
public sealed class BookingDto : IEntityDto
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Начало выполнения
    /// </summary>
    public required DateTimeOffset StartAt { get; init; }

    /// <summary>
    /// Уникальный идентификатор мастера
    /// </summary>
    public required int SpecialistId { get; init; }

    /// <summary>
    /// Мастер
    /// </summary>
    public string? SpecialistName { get; init; }

    /// <summary>
    /// Уникальный идентификатор клиента
    /// </summary>
    public required int CustomerId { get; init; }

    /// <summary>
    /// ФИО клиента
    /// </summary>
    public string? CustomerName { get; init; }

    /// <summary>
    /// Уникальный идентификатор услуги
    /// </summary>
    public required int BeautyServiceId { get; init; }

    /// <summary>
    /// Название услуги
    /// </summary>
    public string? BeautyServiceName { get; init; }

    /// <summary>
    /// Флаг постоянного клиента
    /// </summary>
    public required bool IsRegularCustomer { get; init; }
}