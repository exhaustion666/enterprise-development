using BeautySalon.Application.Mappers;
using BeautySalon.Application.Services;
using BeautySalon.Contracts.Dtos;
using BeautySalon.Contracts.Interfaces;
using BeautySalon.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace BeautySalon.Application.DependencyInjection;

/// <summary>
/// Регистрация сервисов бизнес-логики
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавляет сервисы приложения и мапперы
    /// </summary>
    public static IServiceCollection AddBeautySalonApplication(this IServiceCollection services)
    {
        services.AddSingleton<CustomerMapper>();
        services.AddSingleton<SpecialistMapper>();
        services.AddSingleton<BeautyServiceMapper>();
        services.AddSingleton<BookingMapper>();
        services.AddSingleton<IEntityMapper<Customer, CustomerDto, CustomerEditDto>>(
            provider => provider.GetRequiredService<CustomerMapper>());
        services.AddSingleton<IEntityMapper<Specialist, SpecialistDto, SpecialistEditDto>>(
            provider => provider.GetRequiredService<SpecialistMapper>());
        services.AddSingleton<IEntityMapper<BeautyService, BeautyServiceDto, BeautyServiceEditDto>>(
            provider => provider.GetRequiredService<BeautyServiceMapper>());
        services.AddSingleton<IEntityMapper<Booking, BookingDto, BookingEditDto>>(
            provider => provider.GetRequiredService<BookingMapper>());

        services.AddScoped<ICrudService<CustomerDto, CustomerEditDto>,
            CrudService<Customer, CustomerDto, CustomerEditDto>>();
        services.AddScoped<ICrudService<SpecialistDto, SpecialistEditDto>,
            CrudService<Specialist, SpecialistDto, SpecialistEditDto>>();
        services.AddScoped<ICrudService<BeautyServiceDto, BeautyServiceEditDto>,
            CrudService<BeautyService, BeautyServiceDto, BeautyServiceEditDto>>();
        services.AddScoped<ICrudService<BookingDto, BookingEditDto>,
            CrudService<Booking, BookingDto, BookingEditDto>>();
        services.AddScoped<ISalonAnalyticsService, SalonAnalyticsService>();
        return services;
    }
}