using BeautySalon.Application.Mappers;
using BeautySalon.Contracts.Dtos;
using BeautySalon.Contracts.Interfaces;
using BeautySalon.Domain.Entities;
using BeautySalon.Domain.Interfaces;

namespace BeautySalon.Application.Services;

/// <summary>
/// Класс аналитики по записям салона
/// </summary>
public sealed class SalonAnalyticsService(
    IAnalyticsRepository analyticsRepository,
    IEntityMapper<Specialist, SpecialistDto, SpecialistEditDto> specialistMapper,
    IEntityMapper<Customer, CustomerDto, CustomerEditDto> customerMapper)
    : ISalonAnalyticsService
{
    /// <summary>
    /// Мастера с заданным минимальным стажем
    /// </summary>
    public async Task<IReadOnlyList<SpecialistDto>> GetExperiencedSpecialistsAsync(
        int minimumExperienceYears, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minimumExperienceYears);
        var specialists = await analyticsRepository.GetExperiencedSpecialistsAsync(
            minimumExperienceYears, cancellationToken);
        return [.. specialists.Select(specialistMapper.ToDto)];
    }

    /// <summary>
    /// Свободные "окошки" у мастера
    /// </summary>
    public async Task<IReadOnlyList<TimeSlotDto>?> GetSpecialistWindowsAsync(
        int specialistId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(specialistId);
        var windows = await analyticsRepository.GetSpecialistWindowsAsync(specialistId, cancellationToken);

        return windows is null
            ? null
            : [.. windows.Select(window => new TimeSlotDto
            {
                StartAt = window.StartAt,
                EndAt = window.EndAt,
                Duration = window.Duration
            })];
    }

    /// <summary>
    /// Самые популярные услуги
    /// </summary>
    public async Task<IReadOnlyList<PopularServiceDto>> GetTopServicesAsync(
        int count, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);
        var services = await analyticsRepository.GetTopServicesAsync(count, cancellationToken);
        return [.. services.Select(service => new PopularServiceDto
        {
            BeautyServiceId = service.BeautyServiceId,
            Name = service.Name,
            BookingCount = service.BookingCount
        })];
    }

    /// <summary>
    /// Количество повторных записей клиентов за указанный период
    /// </summary>
    public Task<int> GetRepeatBookingCountAsync(
        DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        if (from >= to)
        {
            throw new ArgumentException("Начало периода должно быть раньше его окончания.");
        }

        return analyticsRepository.GetRepeatBookingCountAsync(from, to, cancellationToken);
    }

    /// <summary>
    /// Клиенты записанные к нескольким мастерам упорядоченные по дате рождения
    /// </summary>
    public async Task<IReadOnlyList<CustomerDto>> GetCustomersWithMultipleSpecialistsAsync(
        CancellationToken cancellationToken = default)
    {
        var customers = await analyticsRepository.GetCustomersWithMultipleSpecialistsAsync(cancellationToken);
        return [.. customers.Select(customerMapper.ToDto)];
    }
}