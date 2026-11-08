using BeautySalon.Contracts.Dtos;
using BeautySalon.Domain.Entities;

namespace BeautySalon.Application.Mappers;

/// <summary>
/// Маппер клиентов
/// </summary>
public sealed class CustomerMapper : IEntityMapper<Customer, CustomerDto, CustomerEditDto>
{
    /// <summary>
    /// Преобразует клиента в DTO
    /// </summary>
    public CustomerDto ToDto(Customer entity) => new()
    {
        Id = entity.Id,
        LastName = entity.LastName,
        FirstName = entity.FirstName,
        Patronymic = entity.Patronymic,
        DateOfBirth = entity.DateOfBirth,
        Gender = entity.Gender,
        Phone = entity.Phone
    };

    /// <summary>
    /// Создаёт доменного клиента из входного DTO
    /// </summary>
    public Customer ToEntity(CustomerEditDto dto, int id) => new()
    {
        Id = id,
        LastName = MappingValidation.Required(dto.LastName, nameof(dto.LastName)),
        FirstName = MappingValidation.Required(dto.FirstName, nameof(dto.FirstName)),
        Patronymic = dto.Patronymic,
        DateOfBirth = MappingValidation.Required(dto.DateOfBirth, nameof(dto.DateOfBirth)),
        Gender = MappingValidation.Required(dto.Gender, nameof(dto.Gender)),
        Phone = MappingValidation.Required(dto.Phone, nameof(dto.Phone))
    };
}