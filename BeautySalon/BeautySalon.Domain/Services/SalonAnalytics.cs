using System;
using System.Collections.Generic;
using System.Text;

using BeautySalon.Domain.Entities;
using BeautySalon.Domain.Models;

namespace BeautySalon.Domain.Services;

public class SalonAnalytics
{
    public IReadOnlyList<Specialist> GetExperiencedSpecialists(
        IEnumerable<Specialist> specialists,
        int minimumExperienceYears)
    {
        ArgumentNullException.ThrowIfNull(specialists);
        ArgumentOutOfRangeException.ThrowIfNegative(minimumExperienceYears);

        return specialists
            .Where(specialist => specialist.ExperienceYears >= minimumExperienceYears)
            .OrderBy(specialist => specialist.Id)
            .ToArray();
    }

    public IReadOnlyList<TimeSlot> GetSpecialistWindows(
        IEnumerable<Booking> bookings,
        int specialistId)
    {
        ArgumentNullException.ThrowIfNull(bookings);

        var specialistBookings = bookings
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
            var currentBooking = specialistBookings[i];
            var nextBooking = specialistBookings[i + 1];
            var windowStart = GetBookingEnd(currentBooking);
            var windowEnd = nextBooking.StartAt;

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

    public IReadOnlyList<PopularService> GetTopServices(
        IEnumerable<Booking> bookings,
        int count)
    {
        ArgumentNullException.ThrowIfNull(bookings);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        return bookings
            .GroupBy(booking => booking.BeautyServiceId)
            .Select(group => new PopularService
            {
                BeautyServiceId = group.Key,
                BookingCount = group.Count()
            })
            .OrderByDescending(service => service.BookingCount)
            .ThenBy(service => service.BeautyServiceId)
            .Take(count)
            .ToArray();
    }

    public int GetRepeatBookingCount(
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

    public IReadOnlyList<Customer> GetCustomersWithMultipleSpecialists(
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

        return customers
            .Where(customer => customerIds.Contains(customer.Id))
            .OrderBy(customer => customer.DateOfBirth)
            .ThenBy(customer => customer.Id)
            .ToArray();
    }

    private static DateTimeOffset GetBookingEnd(Booking booking)
    {
        if (booking.BeautyService is null)
        {
            throw new InvalidOperationException(
                $"У записи {booking.Id} не указана услуга.");
        }

        return booking.StartAt + booking.BeautyService.Duration;
    }
}
