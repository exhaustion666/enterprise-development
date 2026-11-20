using BeautySalon.Domain.Entities;
using BeautySalon.Infrastructure.InMemory;
using BeautySalon.Domain.Interfaces;
using BeautySalon.Domain.Models;

namespace BeautySalon.Infrastructure.InMemory.Repositories;

/// <summary>
/// Репозиторий аналитических запросов
/// </summary>
public sealed class InMemoryAnalyticsRepository(InMemoryStorage storage) : IAnalyticsRepository
{
    /// <summary>
    /// Мастера с заданным минимальным стажем
    /// </summary>
    public async Task<IReadOnlyList<Specialist>> GetExperiencedSpecialistsAsync(
        int minimumExperienceYears,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minimumExperienceYears);

        var specialists = await storage.Specialists.GetAllAsync(cancellationToken);

        return [.. specialists
            .Where(specialist => specialist.ExperienceYears >= minimumExperienceYears)
            .OrderByDescending(specialist => specialist.ExperienceYears)
            .ThenBy(specialist => specialist.Id)];
    }

    /// <summary>
    /// Свободные "окошки" у мастера
    /// </summary>
    public async Task<IReadOnlyList<TimeSlot>?> GetSpecialistWindowsAsync(
        int specialistId,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(specialistId);

        var specialist = await storage.Specialists.GetByIdAsync(
            specialistId, cancellationToken);

        if (specialist is null)
        {
            return null;
        }

        var bookings = await storage.Bookings.GetAllAsync(cancellationToken);
        var specialistBookings = bookings
            .Where(booking => booking.SpecialistId == specialistId)
            .OrderBy(booking => booking.StartAt)
            .ToArray();

        var windows = new List<TimeSlot>();

        if (specialistBookings.Length < 2)
        {
            return windows;
        }

        var firstBooking = specialistBookings[0];
        var previousEnd = firstBooking.StartAt +
            (firstBooking.BeautyService?.Duration
                ?? throw new InvalidOperationException(
                    $"Для записи с ID {firstBooking.Id} не загружена услуга."));

        for (var i = 1; i < specialistBookings.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var currentBooking = specialistBookings[i];

            if (currentBooking.StartAt > previousEnd)
            {
                windows.Add(new TimeSlot
                {
                    StartAt = previousEnd,
                    EndAt = currentBooking.StartAt
                });
            }

            var currentEnd = currentBooking.StartAt +
                (currentBooking.BeautyService?.Duration
                    ?? throw new InvalidOperationException(
                        $"Для записи с ID {currentBooking.Id} не загружена услуга."));

            if (currentEnd > previousEnd)
            {
                previousEnd = currentEnd;
            }
        }

        return windows;
    }

    /// <summary>
    /// Самые популярные услуги
    /// </summary>
    public async Task<IReadOnlyList<PopularService>> GetTopServicesAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        var bookings = await storage.Bookings.GetAllAsync(cancellationToken);

        return [.. bookings
            .GroupBy(booking => booking.BeautyServiceId)
            .Select(group =>
            {
                var beautyService = group.First().BeautyService
                    ?? throw new InvalidOperationException(
                        $"Для записи с ID {group.First().Id} не загружена услуга.");

                return new PopularService
                {
                    BeautyServiceId = group.Key,
                    Name = beautyService.Name,
                    BookingCount = group.Count()
                };
            })
            .OrderByDescending(service => service.BookingCount)
            .ThenBy(service => service.BeautyServiceId)
            .Take(count)];
    }

    /// <summary>
    /// Количество повторных записей клиентов за указанный период
    /// </summary>
    public async Task<int> GetRepeatBookingCountAsync(
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken cancellationToken = default)
    {
        if (from >= to)
        {
            throw new ArgumentException(
                "Начало периода должно быть раньше его окончания.");
        }

        var bookings = await storage.Bookings.GetAllAsync(cancellationToken);

        return bookings.Count(booking =>
            booking.IsRegularCustomer &&
            booking.StartAt >= from &&
            booking.StartAt <= to);
    }

    /// <summary>
    /// Клиенты записанные к нескольким мастерам упорядоченные по дате рождения
    /// </summary>
    public async Task<IReadOnlyList<Customer>> GetCustomersWithMultipleSpecialistsAsync(
        CancellationToken cancellationToken = default)
    {
        var bookings = await storage.Bookings.GetAllAsync(cancellationToken);
        var customerIds = bookings
            .GroupBy(booking => booking.CustomerId)
            .Where(group => group.Select(booking => booking.SpecialistId).Distinct().Count() > 1)
            .Select(group => group.Key)
            .ToHashSet();

        var customers = await storage.Customers.GetAllAsync(cancellationToken);

        return [.. customers
            .Where(customer => customerIds.Contains(customer.Id))
            .OrderBy(customer => customer.DateOfBirth)
            .ThenBy(customer => customer.Id)];
    }
}