using BeautySalon.Domain.Entities;
using BeautySalon.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Infrastructure.InMemory.Repositories;

/// <summary>
/// Репозиторий записей с проверкой и загрузкой связанных клиентов, специалистов и услуг
/// </summary>
public sealed class BookingRepository(
    IEnumerable<Booking> bookings,
    IRepository<Customer, int> customers,
    IRepository<Specialist, int> specialists,
    IRepository<BeautyService, int> beautyServices)
    : InMemoryRepository<Booking>(bookings, UpdateBooking)
{
    /// <summary>
    /// Возвращает все записи
    /// </summary>
    public override async Task<IReadOnlyList<Booking>> GetAllAsync()
    {
        IReadOnlyList<Booking> items = await base.GetAllAsync();

        foreach (Booking booking in items)
        {
            await LoadReferencesAsync(booking);
        }

        return items;
    }

    /// <summary>
    /// Возвращает запись
    /// </summary>
    public override async Task<Booking?> GetByIdAsync(
        int id)
    {
        Booking? booking = await base.GetByIdAsync(id);

        if (booking is not null)
        {
            await LoadReferencesAsync(booking);
        }

        return booking;
    }

    /// <summary>
    /// Проверяет ссылки и добавляет запись
    /// </summary>
    public override async Task<Booking> AddAsync(
        Booking entity)
    {
        await LoadReferencesAsync(entity);

        return await base.AddAsync(entity);
    }

    /// <summary>
    /// Проверяет существование записи и связанные сущности перед обновлением
    /// </summary>
    public override async Task<Booking?> UpdateAsync(
        Booking entity)
    {
        Booking? existing = await base.GetByIdAsync(entity.Id);

        if (existing is null)
        {
            return null;
        }

        await LoadReferencesAsync(entity);

        return await base.UpdateAsync(entity);
    }

    /// <summary>
    /// Проверяет, используется ли клиент в существующих записях
    /// </summary>
    public bool ReferencesCustomer(int customerId)
    {
        return Any(booking => booking.CustomerId == customerId);
    }

    /// <summary>
    /// Проверяет, используется ли мастер в существующих записях
    /// </summary>
    public bool ReferencesSpecialist(int specialistId)
    {
        return Any(booking => booking.SpecialistId == specialistId);
    }

    /// <summary>
    /// Проверяет, используется ли услуга в существующих записях
    /// </summary>
    public bool ReferencesBeautyService(int beautyServiceId)
    {
        return Any(booking => booking.BeautyServiceId == beautyServiceId);
    }

    /// <summary>
    /// Загружает клиента, мастера и услугу, связанные с записью
    /// </summary>
    private async Task LoadReferencesAsync(Booking booking)
    {
        booking.Customer = await customers.GetByIdAsync(booking.CustomerId)
            ?? throw new ValidationException(
                $"Клиент с ID {booking.CustomerId} не существует.");

        booking.Specialist = await specialists.GetByIdAsync(booking.SpecialistId)
            ?? throw new ValidationException(
                $"Специалист с ID {booking.SpecialistId} не существует.");

        booking.BeautyService = await beautyServices.GetByIdAsync(booking.BeautyServiceId)
            ?? throw new ValidationException(
                $"Услуга с ID {booking.BeautyServiceId} не существует.");
    }

    /// <summary>
    /// Переносит данные обновлённой записи в существующий объект
    /// </summary>
    private static void UpdateBooking(Booking existing, Booking updated)
    {
        existing.StartAt = updated.StartAt;
        existing.SpecialistId = updated.SpecialistId;
        existing.Specialist = updated.Specialist;
        existing.CustomerId = updated.CustomerId;
        existing.Customer = updated.Customer;
        existing.BeautyServiceId = updated.BeautyServiceId;
        existing.BeautyService = updated.BeautyService;
        existing.IsRegularCustomer = updated.IsRegularCustomer;
    }
}