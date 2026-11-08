using BeautySalon.Domain.Entities;
using BeautySalon.Domain.Interfaces;
using BeautySalon.Infrastructure.InMemory.Data;
using BeautySalon.Infrastructure.InMemory.Repositories;

namespace BeautySalon.Infrastructure.InMemory;

/// <summary>
/// Общее InMemory-хранилище салона
/// </summary>
public sealed class InMemoryStorage
{
    /// <summary>
    /// Репозиторий клиентов
    /// </summary>
    public IRepository<Customer, int> Customers { get; }

    /// <summary>
    /// Репозиторий мастеров
    /// </summary>
    public IRepository<Specialist, int> Specialists { get; }

    /// <summary>
    /// Репозиторий услуг
    /// </summary>
    public IRepository<BeautyService, int> BeautyServices { get; }

    /// <summary>
    /// Репозиторий записей
    /// </summary>
    public IRepository<Booking, int> Bookings { get; }

    /// <summary>
    /// Создаёт репозитории
    /// </summary>
    public InMemoryStorage()
    {
        BeautySalonSeedData seed = BeautySalonDataSeeder.CreateInitialData();

        BookingRepository? bookingRepository = null;

        Customers = new InMemoryRepository<Customer>(
            seed.Customers,
            UpdateCustomer,
            customer => bookingRepository?.ReferencesCustomer(customer.Id) == true);

        Specialists = new InMemoryRepository<Specialist>(
            seed.Specialists,
            UpdateSpecialist,
            specialist => bookingRepository?.ReferencesSpecialist(specialist.Id) == true);

        BeautyServices = new InMemoryRepository<BeautyService>(
            seed.BeautyServices,
            UpdateBeautyService,
            service => bookingRepository?.ReferencesBeautyService(service.Id) == true);

        bookingRepository = new BookingRepository(
            seed.Bookings,
            Customers,
            Specialists,
            BeautyServices);

        Bookings = bookingRepository;
    }

    /// <summary>
    /// Переносит данные обновлённого клиента в существующий объект
    /// </summary>
    private static void UpdateCustomer(Customer existing, Customer updated)
    {
        existing.LastName = updated.LastName;
        existing.FirstName = updated.FirstName;
        existing.Patronymic = updated.Patronymic;
        existing.DateOfBirth = updated.DateOfBirth;
        existing.Gender = updated.Gender;
        existing.Phone = updated.Phone;
    }

    /// <summary>
    /// Переносит данные обновлённого мастера в существующий объект
    /// </summary>
    private static void UpdateSpecialist(Specialist existing, Specialist updated)
    {
        existing.LastName = updated.LastName;
        existing.FirstName = updated.FirstName;
        existing.Patronymic = updated.Patronymic;
        existing.DateOfBirth = updated.DateOfBirth;
        existing.Gender = updated.Gender;
        existing.Phone = updated.Phone;
        existing.PassportNumber = updated.PassportNumber;
        existing.Specialization = updated.Specialization;
        existing.ExperienceYears = updated.ExperienceYears;
    }

    /// <summary>
    /// Переносит данные обновлённой услуги в существующий объект
    /// </summary>
    private static void UpdateBeautyService(
        BeautyService existing,
        BeautyService updated)
    {
        existing.Name = updated.Name;
        existing.Category = updated.Category;
        existing.Price = updated.Price;
        existing.Duration = updated.Duration;
    }
}