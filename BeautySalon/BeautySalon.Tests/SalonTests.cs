using BeautySalon.Domain.Services;
using BeautySalon.Tests;
using Xunit;

namespace BeautySalon.Tests;

public class SalonTests(SalonFixture fixture)
: IClassFixture<SalonFixture>
{
    private readonly SalonFixture _fixture = fixture;
    private readonly SalonAnalytics _analytics = new();

[Fact]
    public void GetExperiencedSpecialists_WhenExperienceIsAtLeastFiveYears_ReturnsExpectedSpecialists()
    {
        const int minimumExperienceYears = 5;
        int[] expectedIds = [0, 2, 3, 4, 6, 8, 9];

        var specialists = _analytics.GetExperiencedSpecialists(
            _fixture.Specialists,
            minimumExperienceYears);

        Assert.Equal(
            expectedIds,
            specialists.Select(specialist => specialist.Id));
    }

    [Fact]
    public void GetSpecialistWindows_WhenThereAreGaps_ReturnsAllGaps()
    {
        const int specialistId = 0;

        var expectedStarts = new[]
        {
            new DateTimeOffset(2026, 9, 1, 11, 0, 0, TimeSpan.Zero),
        new DateTimeOffset(2026, 9, 1, 14, 0, 0, TimeSpan.Zero)
        };

        var expectedEnds = new[]
        {
            new DateTimeOffset(2026, 9, 1, 11, 30, 0, TimeSpan.Zero),
        new DateTimeOffset(2026, 9, 1, 15, 0, 0, TimeSpan.Zero)
        };

        var windows = _analytics.GetSpecialistWindows(
            _fixture.Bookings,
            specialistId);

        Assert.Equal(
            expectedStarts,
            windows.Select(window => window.StartAt));

        Assert.Equal(
            expectedEnds,
            windows.Select(window => window.EndAt));

        Assert.Equal(
            new[]
            {
            TimeSpan.FromMinutes(30),
            TimeSpan.FromHours(1)
            },
            windows.Select(window => window.Duration));
    }

    [Fact]
    public void GetTopServices_WhenBookingsExist_ReturnsFiveServicesByBookingCount()
    {
        const int count = 5;
        int[] expectedServiceIds = [0, 1, 2, 3, 4];
        int[] expectedBookingCounts = [2, 2, 1, 1, 1];

        var topServices = _analytics.GetTopServices(
            _fixture.Bookings,
            count);

        Assert.Equal(
            expectedServiceIds,
            topServices.Select(service => service.BeautyServiceId));

        Assert.Equal(
            expectedBookingCounts,
            topServices.Select(service => service.BookingCount));
    }

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

        var from = to.AddMonths(-1);

        const int expectedRepeatCount = 3;

        var repeatCount = _analytics.GetRepeatBookingCount(
            _fixture.Bookings,
            from,
            to);

        Assert.Equal(expectedRepeatCount, repeatCount);
    }

    [Fact]
    public void GetCustomersWithMultipleSpecialists_WhenCustomersHaveSeveralSpecialists_ReturnsCustomersByBirthDate()
    {
        int[] expectedCustomerIds = [1, 2];

        var customers = _analytics.GetCustomersWithMultipleSpecialists(
            _fixture.Customers,
            _fixture.Bookings);

        Assert.Equal(
            expectedCustomerIds,
            customers.Select(customer => customer.Id));
    }
}
