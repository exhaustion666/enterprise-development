using BeautySalon.Contracts.Dtos;

namespace BeautySalon.Contracts.Interfaces;

/// <summary>
/// Контракт аналитических запросов салона
/// </summary>
public interface ISalonAnalyticsService
{
    /// <summary>
    /// Мастера с заданным минимальным стажем
    /// </summary>
    public Task<IReadOnlyList<SpecialistDto>> GetExperiencedSpecialistsAsync(
        int minimumExperienceYears);

    /// <summary>
    /// Свободные "окошки" у мастера
    /// </summary>
    public Task<IReadOnlyList<TimeSlotDto>?> GetSpecialistWindowsAsync(
        int specialistId);

    /// <summary>
    /// Самые популярные услуги
    /// </summary>
    public Task<IReadOnlyList<PopularServiceDto>> GetTopServicesAsync(
        int count);

    /// <summary>
    /// Количество повторных записей клиентов за указанный период
    /// </summary>
    public Task<int> GetRepeatBookingCountAsync(
        DateTimeOffset from,
        DateTimeOffset to);

    /// <summary>
    /// Клиенты записанные к нескольким мастерам упорядоченные по дате рождения
    /// </summary>
    public Task<IReadOnlyList<CustomerDto>> GetCustomersWithMultipleSpecialistsAsync();
}