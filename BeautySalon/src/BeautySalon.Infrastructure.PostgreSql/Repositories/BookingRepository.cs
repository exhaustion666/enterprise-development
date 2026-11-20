using BeautySalon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeautySalon.Infrastructure.PostgreSql.Repositories;

/// <summary>
/// Репозиторий записей
/// </summary>
public sealed class BookingRepository(BeautySalonDbContext context)
    : PostgreSqlRepository<Booking>(context)
{
    /// <summary>
    /// Запрос записей с именами клиента, специалиста и услуги
    /// </summary>
    protected override IQueryable<Booking> Query => base.Query
        .Include(booking => booking.Customer)
        .Include(booking => booking.Specialist)
        .Include(booking => booking.BeautyService);
}