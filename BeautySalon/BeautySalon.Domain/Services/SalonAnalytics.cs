using BeautySalon.Domain.Entities;
using BeautySalon.Domain.Models;

namespace BeautySalon.Domain.Services;

public class SalonAnalytics
{
    public static IReadOnlyList<Specialist> GetExperiencedSpecialists(
        IEnumerable<Specialist> specialists,
        int minimumExperienceYears)
    {
        ArgumentNullException.ThrowIfNull(specialists);
        ArgumentOutOfRangeException.ThrowIfNegative(minimumExperienceYears);

        return [.. specialists
            .Where(specialist => specialist.ExperienceYears >= minimumExperienceYears)
            .OrderBy(specialist => specialist.Id)];
    }

    public static IReadOnlyList<TimeSlot> GetSpecialistWindows(
        IEnumerable<Booking> bookings,
        int specialistId)
    {
        ArgumentNullException.ThrowIfNull(bookings);

        Booking[] specialistBookings = bookings
            .Where(booking => booking.SpecialistId == specialistId)
            .OrderBy(booking => booking.StartAt)
            .ToArray();

        if (specialistBookings.Length < 2)
        {
            return [];
        }

        var windows = new List<TimeSlot>();

        for (var i = 0; i < specialistBookings.Length - 1; i++)
        {
            Booking currentBooking = specialistBookings[i];
            Booking nextBooking = specialistBookings[i + 1];
            DateTimeOffset windowStart = GetBookingEnd(currentBooking);
            DateTimeOffset windowEnd = nextBooking.StartAt;

            if (windowStart >= windowEnd || windowStart.Date != windowEnd.Date)
            {
                continue;
            }

            windows.Add(new TimeSlot
            {
                StartAt = windowStart,
                EndAt = windowEnd
            });
        }

        return windows;
    }

    public static IReadOnlyList<PopularService> GetTopServices(
        IEnumerable<Booking> bookings,
        int count)
    {
        ArgumentNullException.ThrowIfNull(bookings);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        return [.. bookings
            .GroupBy(booking => booking.BeautyServiceId)
            .Select(group => new PopularService
            {
                BeautyServiceId = group.Key,
                BookingCount = group.Count()
            })
            .OrderByDescending(service => service.BookingCount)
            .ThenBy(service => service.BeautyServiceId)
            .Take(count)];
    }

    public static int GetRepeatBookingCount(
        IEnumerable<Booking> bookings,
        DateTimeOffset from,
        DateTimeOffset to)
    {
        ArgumentNullException.ThrowIfNull(bookings);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(from, to);

        return bookings
            .Where(booking => booking.StartAt >= from && booking.StartAt <= to)
            .GroupBy(booking => booking.CustomerId)
            .Sum(group => group.Count() > 1 ? group.Count() - 1 : 0);
    }

    public static IReadOnlyList<Customer> GetCustomersWithMultipleSpecialists(
        IEnumerable<Customer> customers,
        IEnumerable<Booking> bookings)
    {
        ArgumentNullException.ThrowIfNull(customers);
        ArgumentNullException.ThrowIfNull(bookings);

        var customerIds = bookings
            .GroupBy(booking => booking.CustomerId)
            .Where(customerBookings => customerBookings
                .Select(booking => booking.SpecialistId)
                .Distinct()
                .Count() > 1)
            .Select(customerBookings => customerBookings.Key)
            .ToHashSet();

        return [.. customers
            .Where(customer => customerIds.Contains(customer.Id))
            .OrderBy(customer => customer.DateOfBirth)
            .ThenBy(customer => customer.Id)];
    }

    private static DateTimeOffset GetBookingEnd(Booking booking)
    {
        return booking.BeautyService is null
            ? throw new InvalidOperationException(
                $"У записи {booking.Id} не указана услуга.")
            : booking.StartAt + booking.BeautyService.Duration;
    }
}
