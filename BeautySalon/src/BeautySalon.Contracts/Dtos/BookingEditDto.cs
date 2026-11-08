using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Contracts.Dtos;

/// <summary>
/// Класс записи на услугу
/// </summary>
public sealed class BookingEditDto
{
    /// <summary>
    /// Начало выполнения
    /// </summary>
    [Required]
    public DateTimeOffset? StartAt { get; init; }

    /// <summary>
    /// Уникальный идентификатор мастера
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    public int? SpecialistId { get; init; }

    /// <summary>
    /// Уникальный идентификатор клиента
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    public int? CustomerId { get; init; }

    /// <summary>
    /// Уникальный идентификатор услуги
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    public int? BeautyServiceId { get; init; }

    /// <summary>
    /// Флаг постоянного клиента
    /// </summary>
    [Required]
    public bool? IsRegularCustomer { get; init; }
}