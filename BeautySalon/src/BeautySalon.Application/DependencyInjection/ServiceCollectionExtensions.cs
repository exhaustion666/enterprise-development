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
        _ = services.AddSingleton<CustomerMapper>();
        _ = services.AddSingleton<SpecialistMapper>();
        _ = services.AddSingleton<BeautyServiceMapper>();
        _ = services.AddSingleton<BookingMapper>();
        _ = services.AddSingleton<IEntityMapper<Customer, CustomerDto, CustomerEditDto>>(
            provider => provider.GetRequiredService<CustomerMapper>());
        _ = services.AddSingleton<IEntityMapper<Specialist, SpecialistDto, SpecialistEditDto>>(
            provider => provider.GetRequiredService<SpecialistMapper>());
        _ = services.AddSingleton<IEntityMapper<BeautyService, BeautyServiceDto, BeautyServiceEditDto>>(
            provider => provider.GetRequiredService<BeautyServiceMapper>());
        _ = services.AddSingleton<IEntityMapper<Booking, BookingDto, BookingEditDto>>(
            provider => provider.GetRequiredService<BookingMapper>());

        _ = services.AddScoped<ICrudService<CustomerDto, CustomerEditDto>,
            CrudService<Customer, CustomerDto, CustomerEditDto>>();
        _ = services.AddScoped<ICrudService<SpecialistDto, SpecialistEditDto>,
            CrudService<Specialist, SpecialistDto, SpecialistEditDto>>();
        _ = services.AddScoped<ICrudService<BeautyServiceDto, BeautyServiceEditDto>,
            CrudService<BeautyService, BeautyServiceDto, BeautyServiceEditDto>>();
        _ = services.AddScoped<ICrudService<BookingDto, BookingEditDto>,
            CrudService<Booking, BookingDto, BookingEditDto>>();
        _ = services.AddScoped<ISalonAnalyticsService, SalonAnalyticsService>();
        return services;
    }
}