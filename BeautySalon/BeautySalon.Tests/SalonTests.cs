using BeautySalon.Domain.Entities;
using BeautySalon.Domain.Models;
using BeautySalon.Domain.Services;
using Xunit;

namespace BeautySalon.Tests;

public class SalonTests(SalonFixture fixture)
: IClassFixture<SalonFixture>
{
    private readonly SalonFixture _fixture = fixture;
    private readonly SalonAnalytics _analytics = new();

    /// <summary>
    /// Мастера стаж работы которых не менее 5 лет
    /// </summary>
    [Fact]
    public void GetExperiencedSpecialists_WhenExperienceIsAtLeastFiveYears_ReturnsExpectedSpecialists()
    {
        const int minimumExperienceYears = 5;
        int[] expectedIds = [0, 2, 3, 4, 6, 8, 9];

        IReadOnlyList<Specialist> specialists = SalonAnalytics.GetExperiencedSpecialists(
            _fixture.Specialists,
            minimumExperienceYears);

        Assert.Equal(
            expectedIds,
            specialists.Select(specialist => specialist.Id));
    }

    /// <summary>
    /// Все "окошки" выбранного мастера
    /// </summary>
    [Fact]
    public void GetSpecialistWindows_WhenThereAreGaps_ReturnsAllGaps()
    {
        const int specialistId = 0;

        DateTimeOffset[] expectedStarts =
        [
            new DateTimeOffset(2026, 9, 1, 11, 0, 0, TimeSpan.Zero),
        new DateTimeOffset(2026, 9, 1, 14, 0, 0, TimeSpan.Zero)
        ];

        DateTimeOffset[] expectedEnds =
        [
            new DateTimeOffset(2026, 9, 1, 11, 30, 0, TimeSpan.Zero),
        new DateTimeOffset(2026, 9, 1, 15, 0, 0, TimeSpan.Zero)
        ];

        IReadOnlyList<TimeSlot> windows = SalonAnalytics.GetSpecialistWindows(
            _fixture.Bookings,
            specialistId);

        Assert.Equal(
            expectedStarts,
            windows.Select(window => window.StartAt));

        Assert.Equal(
            expectedEnds,
            windows.Select(window => window.EndAt));

        Assert.Equal(
            [
            TimeSpan.FromMinutes(30),
            TimeSpan.FromHours(1)
            ],
            windows.Select(window => window.Duration));
    }

    /// <summary>
    /// Топ 5 наиболее популярных услуг
    /// </summary>
    [Fact]
    public void GetTopServices_WhenBookingsExist_ReturnsFiveServicesByBookingCount()
    {
        const int count = 5;
        int[] expectedServiceIds = [0, 1, 2, 3, 4];
        int[] expectedBookingCounts = [2, 2, 1, 1, 1];

        IReadOnlyList<PopularService> topServices = SalonAnalytics.GetTopServices(
            _fixture.Bookings,
            count);

        Assert.Equal(
            expectedServiceIds,
            topServices.Select(service => service.BeautyServiceId));

        Assert.Equal(
            expectedBookingCounts,
            topServices.Select(service => service.BookingCount));
    }

    /// <summary>
    /// Количество повторных записей клиентов за последний месяц
    /// </summary>
    [Fact]
    public void GetRepeatBookingCount_WhenMonthContainsRepeatedCustomers_ReturnsRepeatCount()
    {
        var to = new DateTimeOffset(
            2026,
            9,
            22,
            10,
            0,
            0,
            TimeSpan.Zero);

        DateTimeOffset from = to.AddMonths(-1);

        const int expectedRepeatCount = 3;

        var repeatCount = SalonAnalytics.GetRepeatBookingCount(
            _fixture.Bookings,
            from,
            to);

        Assert.Equal(expectedRepeatCount, repeatCount);
    }

    /// <summary>
    /// Клиенты записанные к нескольким мастерам упорядоченные по дате рождения
    /// </summary>
    [Fact]
    public void GetCustomersWithMultipleSpecialists_WhenCustomersHaveSeveralSpecialists_ReturnsCustomersByBirthDate()
    {
        int[] expectedCustomerIds = [1, 2];

        IReadOnlyList<Customer> customers = SalonAnalytics.GetCustomersWithMultipleSpecialists(
            _fixture.Customers,
            _fixture.Bookings);

        Assert.Equal(
            expectedCustomerIds,
            customers.Select(customer => customer.Id));
    }
}
