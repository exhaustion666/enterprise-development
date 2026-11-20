using BeautySalon.Application.Mappers;
using BeautySalon.Application.Services;
using BeautySalon.Infrastructure.PostgreSql;
using BeautySalon.Infrastructure.PostgreSql.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BeautySalon.Tests;

/// <summary>
/// Класс проверки аналитического сервиса
/// </summary>
public sealed class SalonTests : IDisposable
{
    private readonly BeautySalonDbContext _context;
    private readonly SalonAnalyticsService _analyticsService;

    /// <summary>
    /// Создаёт независимое хранилище
    /// </summary>
    public SalonTests()
    {
        var options = new DbContextOptionsBuilder<BeautySalonDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new BeautySalonDbContext(options);
        _context.Database.EnsureCreated();
        _analyticsService = new SalonAnalyticsService(
            new AnalyticsRepository(_context),
            new SpecialistMapper(),
            new CustomerMapper()
        );
    }

    /// <summary>
    /// Освобождает независимое хранилище
    /// </summary>
    public void Dispose()
    {
        _context.Dispose();
    }

    /// <summary>
    /// Мастера с заданным минимальным стажем
    /// </summary>
    [Fact]
    public async Task GetExperiencedSpecialists_ReturnsExpectedSpecialists()
    {
        var result = await _analyticsService.GetExperiencedSpecialistsAsync(5, TestContext.Current.CancellationToken);

        Assert.Equal([1, 3, 4, 5, 7, 9, 10], result.Select(item => item.Id));
    }

    /// <summary>
    /// Свободные "окошки" у мастера
    /// </summary>
    [Fact]
    public async Task GetSpecialistWindows_ReturnsExpectedTimeSlots()
    {
        var result = await _analyticsService.GetSpecialistWindowsAsync(1, TestContext.Current.CancellationToken);
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
        var result = await _analyticsService.GetTopServicesAsync(5, TestContext.Current.CancellationToken);

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

        var result = await _analyticsService.GetRepeatBookingCountAsync(from, to, TestContext.Current.CancellationToken);

        Assert.Equal(3, result);
    }

    /// <summary>
    /// Клиенты записанные к нескольким мастерам упорядоченные по дате рождения
    /// </summary>
    [Fact]
    public async Task GetCustomersWithMultipleSpecialists_ReturnsExpectedCustomers()
    {
        var result = await _analyticsService.GetCustomersWithMultipleSpecialistsAsync(TestContext.Current.CancellationToken);

        Assert.Equal([2, 3], result.Select(item => item.Id));
    }
}