using BeautySalon.Domain.Entities;
using BeautySalon.Infrastructure.PostgreSql.Data;
using Microsoft.EntityFrameworkCore;

namespace BeautySalon.Infrastructure.PostgreSql;

/// <summary>
/// Контекст базы данных салона красоты
/// </summary>
public sealed class BeautySalonDbContext(DbContextOptions<BeautySalonDbContext> options)
    : DbContext(options)
{
    /// <summary>
    /// Клиенты
    /// </summary>
    public DbSet<Customer> Customers => Set<Customer>();

    /// <summary>
    /// Мастера
    /// </summary>
    public DbSet<Specialist> Specialists => Set<Specialist>();

    /// <summary>
    /// Услуги
    /// </summary>
    public DbSet<BeautyService> BeautyServices => Set<BeautyService>();

    /// <summary>
    /// Записи клиентов
    /// </summary>
    public DbSet<Booking> Bookings => Set<Booking>();

    /// <summary>
    /// Настройка таблицы, ограничений, индексов и начальных данных
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var customers = modelBuilder.Entity<Customer>();
        customers.HasBaseType((Type?)null);
        customers.ToTable("Customers");
        ConfigurePerson(customers);

        var specialists = modelBuilder.Entity<Specialist>();
        specialists.HasBaseType((Type?)null);
        specialists.ToTable("Specialists");
        modelBuilder.Ignore<Person>();
        ConfigurePerson(specialists);
        specialists.Property(specialist => specialist.PassportNumber).HasMaxLength(10);
        specialists.Property(specialist => specialist.Specialization).HasMaxLength(100).IsRequired();
        specialists.Property(specialist => specialist.ExperienceYears).IsRequired();
        specialists.HasIndex(specialist => specialist.PassportNumber).IsUnique();

        modelBuilder.Entity<BeautyService>(entity =>
        {
            entity.ToTable("BeautyServices");
            entity.HasKey(service => service.Id);
            entity.Property(service => service.Name).HasMaxLength(120).IsRequired();
            entity.Property(service => service.Category).HasMaxLength(80).IsRequired();
            entity.Property(service => service.Price).HasPrecision(10, 2).IsRequired();
            entity.Property(service => service.Duration).HasColumnType("interval").IsRequired();
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.ToTable("Bookings");
            entity.HasKey(booking => booking.Id);
            entity.Property(booking => booking.StartAt).HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(booking => booking.IsRegularCustomer).IsRequired();

            entity.HasOne(booking => booking.Customer)
                .WithMany()
                .HasForeignKey(booking => booking.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(booking => booking.Specialist)
                .WithMany()
                .HasForeignKey(booking => booking.SpecialistId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(booking => booking.BeautyService)
                .WithMany()
                .HasForeignKey(booking => booking.BeautyServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(booking => new { booking.SpecialistId, booking.StartAt });
            entity.HasIndex(booking => new { booking.CustomerId, booking.StartAt });
            entity.HasIndex(booking => booking.BeautyServiceId);
        });

        AddInitialData(modelBuilder);
    }

    /// <summary>
    /// Настройка общих свойств клиента или мастера
    /// </summary>
    private static void ConfigurePerson<TEntity>(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TEntity> entity)
        where TEntity : Person
    {
        entity.HasKey(person => person.Id);
        entity.Property(person => person.LastName).HasMaxLength(80).IsRequired();
        entity.Property(person => person.FirstName).HasMaxLength(80).IsRequired();
        entity.Property(person => person.Patronymic).HasMaxLength(80);
        entity.Property(person => person.Phone).HasMaxLength(32).IsRequired();
        entity.Property(person => person.DateOfBirth).IsRequired();
        entity.Property(person => person.Gender).IsRequired();
    }

    /// <summary>
    /// Добавляет исходные записи в модель EF Core
    /// </summary>
    private static void AddInitialData(ModelBuilder modelBuilder)
    {
        var seed = BeautySalonDataSeeder.CreateInitialData();
        modelBuilder.Entity<Customer>().HasData(seed.Customers);
        modelBuilder.Entity<Specialist>().HasData(seed.Specialists);
        modelBuilder.Entity<BeautyService>().HasData(seed.BeautyServices);

        modelBuilder.Entity<Booking>().HasData(seed.Bookings.Select(booking => new Booking
        {
            Id = booking.Id,
            StartAt = booking.StartAt,
            SpecialistId = booking.SpecialistId,
            CustomerId = booking.CustomerId,
            BeautyServiceId = booking.BeautyServiceId,
            IsRegularCustomer = booking.IsRegularCustomer
        }));
    }
}