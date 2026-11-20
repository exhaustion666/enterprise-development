using BeautySalon.Domain.Entities;
using BeautySalon.Domain.Interfaces;
using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BeautySalon.Infrastructure.PostgreSql.Repositories;

/// <summary>
/// Репозиторий аналитики
/// </summary>
public sealed class AnalyticsRepository(BeautySalonDbContext context) : IAnalyticsRepository
{
    /// <summary>
    /// Мастера с заданным минимальным стажем
    /// </summary>
    public async Task<IReadOnlyList<Specialist>> GetExperiencedSpecialistsAsync(
        int minimumExperienceYears,
        CancellationToken cancellationToken = default)
    {
        return await context.Specialists
            .AsNoTracking()
            .Where(specialist => specialist.ExperienceYears >= minimumExperienceYears)
            .OrderBy(specialist => specialist.Id)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Свободные "окошки" у мастера
    /// </summary>
    public async Task<IReadOnlyList<TimeSlot>?> GetSpecialistWindowsAsync(
        int specialistId,
        CancellationToken cancellationToken = default)
    {
        var specialistExists = await context.Specialists
            .AsNoTracking()
            .AnyAsync(specialist => specialist.Id == specialistId, cancellationToken);

        if (!specialistExists)
        {
            return null;
        }

        var bookings = await context.Bookings
            .AsNoTracking()
            .Where(booking => booking.SpecialistId == specialistId)
            .OrderBy(booking => booking.StartAt)
            .ThenBy(booking => booking.Id)
            .Select(booking => new
            {
                booking.StartAt,
                booking.BeautyService!.Duration
            })
            .ToListAsync(cancellationToken);

        var windows = new List<TimeSlot>();

        for (var index = 0; index < bookings.Count - 1; index++)
        {
            var windowStart = bookings[index].StartAt + bookings[index].Duration;
            var windowEnd = bookings[index + 1].StartAt;

            if (windowStart < windowEnd)
            {
                windows.Add(new TimeSlot
                {
                    StartAt = windowStart,
                    EndAt = windowEnd
                });
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
        var query =
            from booking in context.Bookings.AsNoTracking()
            join service in context.BeautyServices.AsNoTracking()
                on booking.BeautyServiceId equals service.Id
            group booking by new { service.Id, service.Name } into serviceBookings
            orderby serviceBookings.Count() descending, serviceBookings.Key.Id
            select new PopularService
            {
                BeautyServiceId = serviceBookings.Key.Id,
                Name = serviceBookings.Key.Name,
                BookingCount = serviceBookings.Count()
            };

        return await query.Take(count).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Количество повторных записей клиентов за указанный период
    /// </summary>
    public async Task<int> GetRepeatBookingCountAsync(
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken cancellationToken = default)
    {
        return await context.Bookings
            .AsNoTracking()
            .Where(booking => booking.StartAt >= from && booking.StartAt <= to)
            .GroupBy(booking => booking.CustomerId)
            .Select(customerBookings => customerBookings.Count() - 1)
            .Where(repeatCount => repeatCount > 0)
            .SumAsync(cancellationToken);
    }

    /// <summary>
    /// Клиенты записанные к нескольким мастерам упорядоченные по дате рождения
    /// </summary>
    public async Task<IReadOnlyList<Customer>> GetCustomersWithMultipleSpecialistsAsync(
        CancellationToken cancellationToken = default)
    {
        var customerIds = context.Bookings
            .AsNoTracking()
            .GroupBy(booking => booking.CustomerId)
            .Where(customerBookings => customerBookings
                .Select(booking => booking.SpecialistId)
                .Distinct()
                .Count() > 1)
            .Select(customerBookings => customerBookings.Key);

        return await context.Customers
            .AsNoTracking()
            .Where(customer => customerIds.Contains(customer.Id))
            .OrderBy(customer => customer.DateOfBirth)
            .ThenBy(customer => customer.Id)
            .ToListAsync(cancellationToken);
    }
}