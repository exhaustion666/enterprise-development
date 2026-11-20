using BeautySalon.Domain.Entities;
using BeautySalon.Domain.Interfaces;
using BeautySalon.Infrastructure.PostgreSql.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BeautySalon.Infrastructure.PostgreSql.DependencyInjection;

/// <summary>
/// Регистрация PostgreSQL-репозиториев
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавляет реализации CRUD и аналитических репозиториев
    /// </summary>
    public static IServiceCollection AddBeautySalonPostgreSqlInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<IRepository<Customer, int>, PostgreSqlRepository<Customer>>();
        services.AddScoped<IRepository<Specialist, int>, PostgreSqlRepository<Specialist>>();
        services.AddScoped<IRepository<BeautyService, int>, PostgreSqlRepository<BeautyService>>();
        services.AddScoped<IRepository<Booking, int>, BookingRepository>();
        services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
        return services;
    }
}