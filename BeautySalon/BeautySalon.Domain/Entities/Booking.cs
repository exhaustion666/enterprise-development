namespace BeautySalon.Domain.Entities;

/// <summary>
/// Класс записи на услугу
/// </summary>
public class Booking
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
    public Specialist? Specialist { get; set; }

    /// <summary>
    /// Уникальный идентификатор клиента
    /// </summary>
    public required int CustomerId { get; set; }

    /// <summary>
    /// Клиент
    /// </summary>
    public Customer? Customer { get; set; }

    /// <summary>
    /// Уникальный идентификатор услуги
    /// </summary>
    public required int BeautyServiceId { get; set; }

    /// <summary>
    /// Услуга
    /// </summary>
    public BeautyService? BeautyService { get; set; }

    /// <summary>
    /// Флаг постоянного клиента
    /// </summary>
    public required bool IsRegularCustomer { get; set; }
}
