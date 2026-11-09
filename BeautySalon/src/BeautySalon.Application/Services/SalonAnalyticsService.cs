using BeautySalon.Application.Analytics;
using BeautySalon.Application.Mappers;
using BeautySalon.Contracts.Dtos;
using BeautySalon.Contracts.Interfaces;
using BeautySalon.Domain.Entities;
using BeautySalon.Domain.Interfaces;
using BeautySalon.Domain.Models;

namespace BeautySalon.Application.Services;

/// <summary>
/// Класс аналитики по записям салона
/// </summary>
public sealed class SalonAnalyticsService(
    IRepository<Specialist, int> specialistRepository,
    IRepository<Customer, int> customerRepository,
    IRepository<Booking, int> bookingRepository,
    IRepository<BeautyService, int> beautyServiceRepository,
    IEntityMapper<Specialist, SpecialistDto, SpecialistEditDto> specialistMapper,
    IEntityMapper<Customer, CustomerDto, CustomerEditDto> customerMapper)
    : ISalonAnalyticsService
{
    /// <summary>
    /// Мастера с заданным минимальным стажем
    /// </summary>
    public async Task<IReadOnlyList<SpecialistDto>> GetExperiencedSpecialistsAsync(
        int minimumExperienceYears)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minimumExperienceYears);

        IReadOnlyList<Specialist> specialists = await specialistRepository.GetAllAsync();

        return [.. SalonAnalytics.GetExperiencedSpecialists(
            specialists,
            minimumExperienceYears).Select(specialistMapper.ToDto)];
    }

    /// <summary>
    /// Свободные "окошки" у мастера
    /// </summary>
    public async Task<IReadOnlyList<TimeSlotDto>?> GetSpecialistWindowsAsync(
        int specialistId)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(specialistId);

        Specialist? specialist = await specialistRepository.GetByIdAsync(
            specialistId);

        if (specialist is null)
        {
            return null;
        }

        IReadOnlyList<Booking> bookings = await bookingRepository.GetAllAsync();

        IReadOnlyList<TimeSlot> windows = SalonAnalytics.GetSpecialistWindows(
            bookings,
            specialistId);

        return [.. windows.Select(window => new TimeSlotDto
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
        int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        IReadOnlyList<Booking> bookings = await bookingRepository.GetAllAsync();
        IReadOnlyList<BeautyService> services = await beautyServiceRepository.GetAllAsync();

        var servicesById = services.ToDictionary(service => service.Id);

        return [.. SalonAnalytics.GetTopServices(bookings, count)
            .Where(popular => servicesById.ContainsKey(popular.BeautyServiceId))
            .Select(popular => new PopularServiceDto
            {
                BeautyServiceId = popular.BeautyServiceId,
                Name = servicesById[popular.BeautyServiceId].Name,
                BookingCount = popular.BookingCount
            })];
    }

    /// <summary>
    /// Количество повторных записей клиентов за указанный период
    /// </summary>
    public async Task<int> GetRepeatBookingCountAsync(
        DateTimeOffset from,
        DateTimeOffset to)
    {
        if (from >= to)
        {
            throw new ArgumentException(
                "Начало периода должно быть раньше его окончания.");
        }

        IReadOnlyList<Booking> bookings = await bookingRepository.GetAllAsync();

        return SalonAnalytics.GetRepeatBookingCount(bookings, from, to);
    }

    /// <summary>
    /// Клиенты записанные к нескольким мастерам упорядоченные по дате рождения
    /// </summary>
    public async Task<IReadOnlyList<CustomerDto>> GetCustomersWithMultipleSpecialistsAsync()
    {
        IReadOnlyList<Customer> customers = await customerRepository.GetAllAsync();
        IReadOnlyList<Booking> bookings = await bookingRepository.GetAllAsync();

        return [.. SalonAnalytics.GetCustomersWithMultipleSpecialists(
            customers,
            bookings).Select(customerMapper.ToDto)];
    }
}