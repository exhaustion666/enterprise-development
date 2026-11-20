namespace BeautySalon.Contracts.Dtos;

/// <summary>
/// Класс записи на услугу
/// </summary>
public sealed class BookingDto : IEntityDto
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Начало выполнения
    /// </summary>
    public required DateTimeOffset StartAt { get; set; }

    /// <summary>
    /// Уникальный идентификатор мастера
    /// </summary>
    public required int SpecialistId { get; set; }

    /// <summary>
    /// Мастер
    /// </summary>
    public string? SpecialistName { get; set; }

    /// <summary>
    /// Уникальный идентификатор клиента
    /// </summary>
    public required int CustomerId { get; set; }

    /// <summary>
    /// ФИО клиента
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Уникальный идентификатор услуги
    /// </summary>
    public required int BeautyServiceId { get; set; }

    /// <summary>
    /// Название услуги
    /// </summary>
    public string? BeautyServiceName { get; set; }

    /// <summary>
    /// Флаг постоянного клиента
    /// </summary>
    public required bool IsRegularCustomer { get; set; }
}