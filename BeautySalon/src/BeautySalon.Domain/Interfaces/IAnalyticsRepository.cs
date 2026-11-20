using BeautySalon.Domain.Entities;
using BeautySalon.Domain.Models;

namespace BeautySalon.Domain.Interfaces;

/// <summary>
/// Контракт репозитория аналитических запросов
/// </summary>
public interface IAnalyticsRepository
{
    /// <summary>
    /// Мастера с заданным минимальным стажем
    /// </summary>
    public Task<IReadOnlyList<Specialist>> GetExperiencedSpecialistsAsync(
        int minimumExperienceYears, CancellationToken cancellationToken = default);

    /// <summary>
    /// Свободные "окошки" у мастера
    /// </summary>
    public Task<IReadOnlyList<TimeSlot>?> GetSpecialistWindowsAsync(
        int specialistId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Самые популярные услуги
    /// </summary>
    public Task<IReadOnlyList<PopularService>> GetTopServicesAsync(
        int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Количество повторных записей клиентов за указанный период
    /// </summary>
    public Task<int> GetRepeatBookingCountAsync(
        DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);

    /// <summary>
    /// Клиенты записанные к нескольким мастерам упорядоченные по дате рождения
    /// </summary>
    public Task<IReadOnlyList<Customer>> GetCustomersWithMultipleSpecialistsAsync(
        CancellationToken cancellationToken = default);
}