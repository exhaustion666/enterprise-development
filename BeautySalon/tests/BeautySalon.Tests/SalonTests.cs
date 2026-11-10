using BeautySalon.Application.Mappers;
using BeautySalon.Application.Services;
using BeautySalon.Contracts.Dtos;
using BeautySalon.Infrastructure.InMemory;
using Xunit;

namespace BeautySalon.Tests;

/// <summary>
/// Класс проверки аналитического сервиса
/// </summary>
public sealed class SalonTests
{
    private readonly SalonAnalyticsService _analyticsService;

    /// <summary>
    /// Создаёт независимое хранилище
    /// </summary>
    public SalonTests()
    {
        var storage = new InMemoryStorage();
        _analyticsService = new SalonAnalyticsService(
            storage.Specialists,
            storage.Customers,
            storage.Bookings,
            storage.BeautyServices,
            new SpecialistMapper(),
            new CustomerMapper()
        );
    }

    /// <summary>
    /// Мастера с заданным минимальным стажем
    /// </summary>
    [Fact]
    public async Task GetExperiencedSpecialists_ReturnsExpectedSpecialists()
    {
        var result = await _analyticsService.GetExperiencedSpecialistsAsync(5);

        Assert.Equal([1, 3, 4, 5, 7, 9, 10], result.Select(item => item.Id));
    }

    /// <summary>
    /// Свободные "окошки" у мастера
    /// </summary>
    [Fact]
    public async Task GetSpecialistWindows_ReturnsExpectedTimeSlots()
    {
        var result = await _analyticsService.GetSpecialistWindowsAsync(1);
        var windows = result ?? throw new InvalidOperationException("Для существующего специалиста должен возвращаться список окон.");
        Assert.Equal(2, windows.Count);
        Assert.Equal(
            new DateTimeOffset(2026, 9, 1, 11, 0, 0, TimeSpan.Zero),
            windows[0].StartAt);
        Assert.Equal(
            new DateTimeOffset(2026, 9, 1, 11, 30, 0, TimeSpan.Zero),
            windows[0].EndAt);
        Assert.Equal(
            new DateTimeOffset(2026, 9, 1, 14, 0, 0, TimeSpan.Zero),
            windows[1].StartAt);
        Assert.Equal(
            new DateTimeOffset(2026, 9, 1, 15, 0, 0, TimeSpan.Zero),
            windows[1].EndAt);
    }

    /// <summary>
    /// Самые популярные услуги
    /// </summary>
    [Fact]
    public async Task GetTopServices_ReturnsExpectedOrder()
    {
        var result = await _analyticsService.GetTopServicesAsync(5);

        Assert.Equal([1, 2, 3, 4, 5], result.Select(item => item.BeautyServiceId));
        Assert.Equal([2, 2, 1, 1, 1], result.Select(item => item.BookingCount));
    }

    /// <summary>
    /// Количество повторных записей клиентов за указанный период
    /// </summary>
    [Fact]
    public async Task GetRepeatBookingCount_ReturnsExpectedCount()
    {
        var from = new DateTimeOffset(2026, 9, 1, 10, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2026, 9, 22, 10, 0, 0, TimeSpan.Zero);

        var result = await _analyticsService.GetRepeatBookingCountAsync(from, to);

        Assert.Equal(3, result);
    }

    /// <summary>
    /// Клиенты записанные к нескольким мастерам упорядоченные по дате рождения
    /// </summary>
    [Fact]
    public async Task GetCustomersWithMultipleSpecialists_ReturnsExpectedCustomers()
    {
        var result = await _analyticsService.GetCustomersWithMultipleSpecialistsAsync();

        Assert.Equal([2, 3], result.Select(item => item.Id));
    }
}