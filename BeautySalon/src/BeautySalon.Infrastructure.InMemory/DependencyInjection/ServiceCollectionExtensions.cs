using BeautySalon.Domain.Entities;
using BeautySalon.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BeautySalon.Infrastructure.InMemory.DependencyInjection;

/// <summary>
/// Регистрация InMemory-хранилища и репозиториев
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавляет единое хранилище и его репозитории в контейнер зависимостей
    /// </summary>
    public static IServiceCollection AddBeautySalonInMemoryInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryStorage>();
        services.AddSingleton<IRepository<Customer, int>>(provider => provider.GetRequiredService<InMemoryStorage>().Customers);
        services.AddSingleton<IRepository<Specialist, int>>(provider => provider.GetRequiredService<InMemoryStorage>().Specialists);
        services.AddSingleton<IRepository<BeautyService, int>>(provider => provider.GetRequiredService<InMemoryStorage>().BeautyServices);
        services.AddSingleton<IRepository<Booking, int>>(provider => provider.GetRequiredService<InMemoryStorage>().Bookings);
        return services;
    }
}