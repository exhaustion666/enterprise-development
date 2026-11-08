using BeautySalon.Contracts.Dtos;
using BeautySalon.Domain.Entities;

namespace BeautySalon.Application.Mappers;

/// <summary>
/// Маппер записей
/// </summary>
public sealed class BookingMapper : IEntityMapper<Booking, BookingDto, BookingEditDto>
{
    /// <summary>
    /// Преобразует запись и загруженные связанные данные в DTO
    /// </summary>
    public BookingDto ToDto(Booking entity) => new()
    {
        Id = entity.Id,
        StartAt = entity.StartAt,
        SpecialistId = entity.SpecialistId,
        SpecialistName = entity.Specialist is null ? null : FullName(entity.Specialist),
        CustomerId = entity.CustomerId,
        CustomerName = entity.Customer is null ? null : FullName(entity.Customer),
        BeautyServiceId = entity.BeautyServiceId,
        BeautyServiceName = entity.BeautyService?.Name,
        IsRegularCustomer = entity.IsRegularCustomer
    };

    /// <summary>
    /// Создаёт доменную запись из входного DTO
    /// </summary>
    public Booking ToEntity(BookingEditDto dto, int id) => new()
    {
        Id = id,
        StartAt = MappingValidation.Required(dto.StartAt, nameof(dto.StartAt)).ToUniversalTime(),
        SpecialistId = MappingValidation.Required(dto.SpecialistId, nameof(dto.SpecialistId)),
        CustomerId = MappingValidation.Required(dto.CustomerId, nameof(dto.CustomerId)),
        BeautyServiceId = MappingValidation.Required(dto.BeautyServiceId, nameof(dto.BeautyServiceId)),
        IsRegularCustomer = MappingValidation.Required(dto.IsRegularCustomer, nameof(dto.IsRegularCustomer))
    };

    /// <summary>
    /// Формирует полное имя человека пропуская отсутствующие части имени
    /// </summary>
    private static string FullName(Person person) =>
        string.Join(" ", new[] { person.LastName, person.FirstName, person.Patronymic }
            .Where(part => !string.IsNullOrWhiteSpace(part)));
}